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
        public static readonly byte BlockID = 0x3D;

        public override byte ID { get { return 0x3D; } }

        public override double BlastResistance { get { return 17.5; } }

        public override double Hardness { get { return 3.5; } }

        public override byte Luminance { get { return 0; } }

        public override string DisplayName { get { return "Furnace"; } }

        protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
        {
            return new ItemStack[] { new ItemStack(BlockID) };
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

            var entity = world.GetTileEntity(descriptor.Coordinates);
            if (entity != null)
            {
                int i = 0;
                foreach (var item in (NbtList)entity["Items"])
                {
                    var slot = ItemStack.FromNbt((NbtCompound)item);
                    window[i++] = slot;
                }
            }

            user.OpenWindow(window);

            if (entity != null)
            {
                var burnTime = entity["BurnTime"].ShortValue;
                var burnTotal = entity["BurnTotal"].ShortValue;
                var cookTime = entity["CookTime"].ShortValue;
                var burnProgress = (short)(((double)burnTime / burnTotal) * 250);
                if (burnTime == 0)
                    burnProgress = 0;
                if (cookTime != 0)
                    window.Client.QueuePacket(new UpdateProgressPacket(window.ID,
                        UpdateProgressPacket.ProgressTarget.ItemCompletion, cookTime));
                if (burnProgress != 0)
                    window.Client.QueuePacket(new UpdateProgressPacket(window.ID,
                            UpdateProgressPacket.ProgressTarget.AvailableHeat, burnProgress));
            }

            window.WindowChange += (sender, e) => FurnaceWindowChanged(sender, e, world);
            return false;
        }

        private bool Handling = false;

        private NbtCompound CreateTileEntity()
        {
            return new NbtCompound(new NbtTag[]
            {
                new NbtShort("BurnTime", 0),
                new NbtShort("BurnTotal", 0),
                new NbtShort("CookTime", 20),
                new NbtList("Items", new[]
                {
                    ItemStack.EmptyStack.ToNbt(),
                    ItemStack.EmptyStack.ToNbt(),
                    ItemStack.EmptyStack.ToNbt()
                }, NbtTagType.Compound)
            });
        }

        protected void FurnaceWindowChanged(object sender, WindowChangeEventArgs e, IWorld world)
        {
            if (Handling)
                return;
            var window = sender as FurnaceWindow;
            var index = e.SlotIndex;
            if (index >= FurnaceWindow.MainIndex)
                return;

            Handling =  true;
            window[index] = e.Value;

            var entity = world.GetTileEntity(window.Coordinates);
            if (entity == null)
                entity = CreateTileEntity();

            entity["Items"] = new NbtList("Items", new NbtTag[]
            {
                window[0].ToNbt(), window[1].ToNbt(), window[2].ToNbt()
            }, NbtTagType.Compound);

            world.SetTileEntity(window.Coordinates, entity);

            UpdateFurnaceState(window.EventScheduler, world, entity, window.ItemRepository, window.Coordinates, window, TimeSpan.Zero);

            Handling = false;
        }

        private void UpdateFurnaceState(IEventScheduler scheduler, IWorld world, NbtCompound tileEntity,
                                        IItemRepository itemRepository, Coordinates3D coords, FurnaceWindow window, TimeSpan elapsed)
        {
            if (world.GetBlockID(coords) != FurnaceBlock.BlockID && world.GetBlockID(coords) != LitFurnaceBlock.BlockID)
            {
                if (window != null && !window.IsDisposed)
                    window.Dispose();
                return;
            }
            // TODO
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