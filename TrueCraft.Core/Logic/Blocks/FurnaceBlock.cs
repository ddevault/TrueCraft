using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.API.Networking;
using TrueCraft.API.World;
using TrueCraft.Core.Windows;
using TrueCraft.API.Windows;
using System.Collections.Generic;
using fNbt;
using TrueCraft.API.Server;
using TrueCraft.Core.Networking.Packets;
using TrueCraft.Core.Entities;

namespace TrueCraft.Core.Logic.Blocks
{
    public class FurnaceBlock : BlockProvider, ICraftingRecipe
    {
        protected class FurnaceState
        {
            public short BurnTimeRemaining { get; set; }
            public short BurnTimeTotal { get; set; }
            public short CookTime { get; set; }
            public ItemStack[] Items { get; set; }

            public FurnaceState()
            {
                Items = new ItemStack[3];
            }
        }

        protected class FurnaceEventSubject : IEventSubject
        {
            public event EventHandler Disposed;

            public void Dispose()
            {
                if (Disposed != null)
                    Dispose();
            }
        }

        public static readonly byte BlockID = 0x3D;

        public override byte ID { get { return 0x3D; } }

        public override double BlastResistance { get { return 17.5; } }

        public override double Hardness { get { return 3.5; } }

        public override byte Luminance { get { return 0; } }

        public override string DisplayName { get { return "Furnace"; } }

        protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
        {
            return new[] { new ItemStack(BlockID) };
        }

        protected static Dictionary<Coordinates3D, FurnaceEventSubject> TrackedFurnaces { get; set; }
        protected static Dictionary<Coordinates3D, List<IWindow>> TrackedFurnaceWindows { get; set; }

        public FurnaceBlock()
        {
            TrackedFurnaces = new Dictionary<Coordinates3D, FurnaceEventSubject>();
            TrackedFurnaceWindows = new Dictionary<Coordinates3D, List<IWindow>>();
        }

        private NbtCompound CreateTileEntity()
        {
            return new NbtCompound(new NbtTag[]
            {
                new NbtShort("BurnTime", 0),
                new NbtShort("BurnTotal", 0),
                new NbtShort("CookTime", -1),
                new NbtList("Items", new[]
                {
                    ItemStack.EmptyStack.ToNbt(),
                    ItemStack.EmptyStack.ToNbt(),
                    ItemStack.EmptyStack.ToNbt()
                }, NbtTagType.Compound)
            });
        }

        private FurnaceState GetState(IWorld world, Coordinates3D coords)
        {
            var tileEntity = world.GetTileEntity(coords);
            if (tileEntity == null)
                tileEntity = CreateTileEntity();
            var burnTime = tileEntity.Get<NbtShort>("BurnTime");
            var burnTotal = tileEntity.Get<NbtShort>("BurnTotal");
            var cookTime = tileEntity.Get<NbtShort>("CookTime");
            var state = new FurnaceState
            {
                BurnTimeTotal = burnTotal == null ? (short)0 : burnTotal.Value,
                BurnTimeRemaining = burnTime == null ? (short)0 : burnTime.Value,
                CookTime = cookTime == null ? (short)200 : cookTime.Value
            };
            var items = tileEntity.Get<NbtList>("Items");
            if (items != null)
            {
                int i = 0;
                foreach (var item in items)
                    state.Items[i++] = ItemStack.FromNbt((NbtCompound)item);
            }
            return state;
        }

        private void UpdateWindows(Coordinates3D coords, FurnaceState state)
        {
            if (TrackedFurnaceWindows.ContainsKey(coords))
            {
                Handling = true;
                foreach (var window in TrackedFurnaceWindows[coords])
                {
                    window[0] = state.Items[0];
                    window[1] = state.Items[1];
                    window[2] = state.Items[2];

                    window.Client.QueuePacket(new UpdateProgressPacket(
                        window.ID, UpdateProgressPacket.ProgressTarget.ItemCompletion, state.CookTime));
                    var burnProgress = state.BurnTimeRemaining / (double)state.BurnTimeTotal;
                    var burn = (short)(burnProgress * 250);
                    window.Client.QueuePacket(new UpdateProgressPacket(
                        window.ID, UpdateProgressPacket.ProgressTarget.AvailableHeat, burn));
                }
                Handling = false;
            }
        }

        private void SetState(IWorld world, Coordinates3D coords, FurnaceState state)
        {
            world.SetTileEntity(coords, new NbtCompound(new NbtTag[]
            {
                new NbtShort("BurnTime", state.BurnTimeRemaining),
                new NbtShort("BurnTotal", state.BurnTimeTotal),
                new NbtShort("CookTime", state.CookTime),
                new NbtList("Items", new[]
                {
                    state.Items[0].ToNbt(),
                    state.Items[1].ToNbt(),
                    state.Items[2].ToNbt()
                }, NbtTagType.Compound)
            }));
            UpdateWindows(coords, state);
        }

        public override void BlockMined(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            var entity = world.GetTileEntity(descriptor.Coordinates);
            if (entity != null)
            {
                foreach (var item in (NbtList)entity["Items"])
                {
                    var manager = user.Server.GetEntityManagerForWorld(world);
                    var slot = ItemStack.FromNbt((NbtCompound)item);
                    manager.SpawnEntity(new ItemEntity(descriptor.Coordinates + new Vector3(0.5), slot));
                }
                world.SetTileEntity(descriptor.Coordinates, null);
            }
            base.BlockMined(descriptor, face, world, user);
        }

        public override bool BlockRightClicked(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            var window = new FurnaceWindow(user.Server.Scheduler, descriptor.Coordinates,
                             user.Server.ItemRepository, (InventoryWindow)user.Inventory);

            var state = GetState(world, descriptor.Coordinates);
            for (int i = 0; i < state.Items.Length; i++)
                window[i] = state.Items[i];

            user.OpenWindow(window);
            if (!TrackedFurnaceWindows.ContainsKey(descriptor.Coordinates))
                TrackedFurnaceWindows[descriptor.Coordinates] = new List<IWindow>();
            TrackedFurnaceWindows[descriptor.Coordinates].Add(window);
            window.Disposed += (sender, e) => TrackedFurnaceWindows.Remove(descriptor.Coordinates);
            UpdateWindows(descriptor.Coordinates, state);

            // TODO: Set window progress appropriately

            window.WindowChange += (sender, e) => FurnaceWindowChanged(sender, e, world);
            return false;
        }

        private bool Handling = false;

        protected void FurnaceWindowChanged(object sender, WindowChangeEventArgs e, IWorld world)
        {
            if (Handling)
                return;
            var window = sender as FurnaceWindow;
            var index = e.SlotIndex;
            if (index >= FurnaceWindow.MainIndex)
                return;

            Handling = true;
            e.Handled = true;
            window[index] = e.Value;

            var state = GetState(world, window.Coordinates);

            state.Items[0] = window[0];
            state.Items[1] = window[1];
            state.Items[2] = window[2];

            SetState(world, window.Coordinates, state);

            Handling = true;

            if (!TrackedFurnaces.ContainsKey(window.Coordinates))
            {
                // Set up the initial state
                TryInitializeFurnace(state, window.EventScheduler, world, window.Coordinates, window.ItemRepository);
            }

            Handling = false;
        }

        private void TryInitializeFurnace(FurnaceState state, IEventScheduler scheduler, IWorld world,
                                          Coordinates3D coords, IItemRepository itemRepository)
        {
            if (TrackedFurnaces.ContainsKey(coords))
                return;

            var inputStack = state.Items[FurnaceWindow.IngredientIndex];
            var fuelStack = state.Items[FurnaceWindow.FuelIndex];
            var outputStack = state.Items[FurnaceWindow.OutputIndex];

            var input = itemRepository.GetItemProvider(inputStack.ID) as ISmeltableItem;
            var fuel = itemRepository.GetItemProvider(fuelStack.ID) as IBurnableItem;

            if (state.BurnTimeRemaining > 0)
            {
                if (state.CookTime == -1 && input != null && (outputStack.Empty || outputStack.CanMerge(input.SmeltingOutput)))
                {
                    state.CookTime = 0;
                    SetState(world, coords, state);
                }
                var subject = new FurnaceEventSubject();
                TrackedFurnaces[coords] = subject;
                scheduler.ScheduleEvent("smelting", subject, TimeSpan.FromSeconds(1),
                    server => UpdateFurnace(server.Scheduler, world, coords, itemRepository));
                return;
            }

            if (fuel != null && input != null) // We can maybe start
            {
                if (outputStack.Empty || outputStack.CanMerge(input.SmeltingOutput))
                {
                    // We can definitely start
                    state.BurnTimeRemaining = state.BurnTimeTotal = (short)(fuel.BurnTime.TotalSeconds * 20);
                    state.CookTime = 0;
                    state.Items[FurnaceWindow.FuelIndex].Count--;
                    SetState(world, coords, state);
                    world.SetBlockID(coords, LitFurnaceBlock.BlockID);
                    var subject = new FurnaceEventSubject();
                    TrackedFurnaces[coords] = subject;
                    scheduler.ScheduleEvent("smelting", subject, TimeSpan.FromSeconds(1),
                        server => UpdateFurnace(server.Scheduler, world, coords, itemRepository));
                }
            }
        }

        private void UpdateFurnace(IEventScheduler scheduler, IWorld world, Coordinates3D coords, IItemRepository itemRepository)
        {
            if (TrackedFurnaces.ContainsKey(coords))
                TrackedFurnaces.Remove(coords);

            if (world.GetBlockID(coords) != FurnaceBlock.BlockID && world.GetBlockID(coords) != LitFurnaceBlock.BlockID)
            {
                /*if (window != null && !window.IsDisposed)
                    window.Dispose();*/
                return;
            }

            var state = GetState(world, coords);

            var inputStack = state.Items[FurnaceWindow.IngredientIndex];
            var outputStack = state.Items[FurnaceWindow.OutputIndex];

            var input = itemRepository.GetItemProvider(inputStack.ID) as ISmeltableItem;

            // Update burn time
            var burnTime = state.BurnTimeRemaining;
            if (state.BurnTimeRemaining > 0)
            {
                state.BurnTimeRemaining -= 20; // ticks
                if (state.BurnTimeRemaining <= 0)
                {
                    state.BurnTimeRemaining = 0;
                    state.BurnTimeTotal = 0;
                    world.SetBlockID(coords, FurnaceBlock.BlockID);
                }
            }

            // Update cook time
            if (state.CookTime < 200 && state.CookTime >= 0)
            {
                state.CookTime += 20; // ticks
                if (state.CookTime >= 200)
                    state.CookTime = 200;
            }

            // Are we done cooking?
            if (state.CookTime == 200 && burnTime > 0)
            {
                state.CookTime = -1;
                if (input != null && (outputStack.Empty || outputStack.CanMerge(input.SmeltingOutput)))
                {
                    if (outputStack.Empty)
                        outputStack = input.SmeltingOutput;
                    else if (outputStack.CanMerge(input.SmeltingOutput))
                        outputStack.Count += input.SmeltingOutput.Count;
                    state.Items[FurnaceWindow.OutputIndex] = outputStack;
                    state.Items[FurnaceWindow.IngredientIndex].Count--;
                }
            }

            SetState(world, coords, state);
            TryInitializeFurnace(state, scheduler, world, coords, itemRepository);
        }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(13, 2);
        }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    {
                        new ItemStack(CobblestoneBlock.BlockID),
                        new ItemStack(CobblestoneBlock.BlockID),
                        new ItemStack(CobblestoneBlock.BlockID)
                    },
                    {
                        new ItemStack(CobblestoneBlock.BlockID),
                        ItemStack.EmptyStack,
                        new ItemStack(CobblestoneBlock.BlockID)
                    },
                    {
                        new ItemStack(CobblestoneBlock.BlockID),
                        new ItemStack(CobblestoneBlock.BlockID),
                        new ItemStack(CobblestoneBlock.BlockID)
                    }
                };
            }
        }

        public ItemStack Output
        {
            get { return new ItemStack(BlockID); }
        }

        public bool SignificantMetadata
        {
            get { return false; }
        }

        public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            world.SetMetadata(descriptor.Coordinates, (byte)MathHelper.DirectionByRotationFlat(user.Entity.Yaw, true));
        }
    }

    public class LitFurnaceBlock : FurnaceBlock
    {
        public static readonly new byte BlockID = 0x3E;

        public override byte ID { get { return 0x3E; } }

        public override byte Luminance { get { return 13; } }

        public override bool Opaque { get { return false; } }

        public override string DisplayName { get { return "Furnace (lit)"; } }
    }
}