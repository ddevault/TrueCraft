using System;
using TrueCraft.API;
using TrueCraft.Core.Logic.Items;
using TrueCraft.API.Logic;
using TrueCraft.API.World;
using TrueCraft.API.Networking;
using fNbt;
using TrueCraft.Core.Windows;
using System.Collections.Generic;
using TrueCraft.Core.Entities;

namespace TrueCraft.Core.Logic.Blocks
{
    public class ChestBlock : BlockProvider, ICraftingRecipe
    {
        public static readonly byte BlockID = 0x36;
        
        public override byte ID { get { return 0x36; } }
        
        public override double BlastResistance { get { return 12.5; } }

        public override double Hardness { get { return 2.5; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }
        
        public override string DisplayName { get { return "Chest"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(10, 1);
        }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    {
                        new ItemStack(WoodenPlanksBlock.BlockID),
                        new ItemStack(WoodenPlanksBlock.BlockID),
                        new ItemStack(WoodenPlanksBlock.BlockID)
                    },
                    {
                        new ItemStack(WoodenPlanksBlock.BlockID),
                        ItemStack.EmptyStack,
                        new ItemStack(WoodenPlanksBlock.BlockID)
                    },
                    {
                        new ItemStack(WoodenPlanksBlock.BlockID),
                        new ItemStack(WoodenPlanksBlock.BlockID),
                        new ItemStack(WoodenPlanksBlock.BlockID)
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

        private static readonly Coordinates3D[] AdjacentBlocks =
        {
            Coordinates3D.North,
            Coordinates3D.South,
            Coordinates3D.West,
            Coordinates3D.East
        };

        public override void ItemUsedOnBlock(Coordinates3D coordinates, ItemStack item, BlockFace face, IWorld world, IRemoteClient user)
        {
            int adjacent = 0;
            var coords = coordinates + MathHelper.BlockFaceToCoordinates(face);
            Coordinates3D _ = Coordinates3D.Down;
            // Check for adjacent chests. We can only allow one adjacent check block.
            for (int i = 0; i < AdjacentBlocks.Length; i++)
            {
                if (world.GetBlockID(coords + AdjacentBlocks[i]) == ChestBlock.BlockID)
                {
                    _ = coords + AdjacentBlocks[i];
                    adjacent++;
                }
            }
            if (adjacent <= 1)
            {
                if (_ != Coordinates3D.Down)
                {
                    // Confirm that adjacent chest is not a double chest
                    for (int i = 0; i < AdjacentBlocks.Length; i++)
                    {
                        if (world.GetBlockID(_ + AdjacentBlocks[i]) == ChestBlock.BlockID)
                            adjacent++;
                    }
                }
                if (adjacent <= 1)
                    base.ItemUsedOnBlock(coordinates, item, face, world, user);
            }
        }

        public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            world.SetMetadata(descriptor.Coordinates, (byte)MathHelper.DirectionByRotationFlat(user.Entity.Yaw, true));
        }

        public override bool BlockRightClicked(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            var adjacent = -Coordinates3D.One; // -1, no adjacent chest
            var self = descriptor.Coordinates;
            for (int i = 0; i < AdjacentBlocks.Length; i++)
            {
                var test = self + AdjacentBlocks[i];
                if (world.GetBlockID(test) == ChestBlock.BlockID)
                {
                    adjacent = test;
                    var up = world.BlockRepository.GetBlockProvider(world.GetBlockID(test + Coordinates3D.Up));
                    if (up.Opaque && !(up is WallSignBlock)) // Wall sign blocks are an exception
                        return false; // Obstructed
                    break;
                }
            }
            var upSelf = world.BlockRepository.GetBlockProvider(world.GetBlockID(self + Coordinates3D.Up));
            if (upSelf.Opaque && !(upSelf is WallSignBlock))
                return false; // Obstructed

            if (adjacent != -Coordinates3D.One)
            {
                // Ensure that chests are always opened in the same arrangement
                if (adjacent.X < self.X ||
                    adjacent.Z < self.Z)
                {
                    var _ = adjacent;
                    adjacent = self;
                    self = _; // Swap
                }
            }

            var window = new ChestWindow((InventoryWindow)user.Inventory, adjacent != -Coordinates3D.One);
            // Add items
            var entity = world.GetTileEntity(self);
            if (entity != null)
            {
                foreach (var item in (NbtList)entity["Items"])
                {
                    var slot = ItemStack.FromNbt((NbtCompound)item);
                    window.ChestInventory[slot.Index] = slot;
                }
            }
            // Add adjacent items
            if (adjacent != -Coordinates3D.One)
            {
                entity = world.GetTileEntity(adjacent);
                if (entity != null)
                {
                    foreach (var item in (NbtList)entity["Items"])
                    {
                        var slot = ItemStack.FromNbt((NbtCompound)item);
                        window.ChestInventory[slot.Index + ChestWindow.DoubleChestSecondaryIndex] = slot;
                    }
                }
            }
            window.WindowChange += (sender, e) =>
                {
                    var entitySelf = new NbtList("Items", NbtTagType.Compound);
                    var entityAdjacent = new NbtList("Items", NbtTagType.Compound);
                    for (int i = 0; i < window.ChestInventory.Items.Length; i++)
                    {
                        var item = window.ChestInventory.Items[i];
                        if (!item.Empty)
                        {
                            if (i < ChestWindow.DoubleChestSecondaryIndex)
                            {
                                item.Index = i;
                                entitySelf.Add(item.ToNbt());
                            }
                            else
                            {
                                item.Index = i - ChestWindow.DoubleChestSecondaryIndex;
                                entityAdjacent.Add(item.ToNbt());
                            }
                        }
                    }
                    var newEntity = world.GetTileEntity(self);
                    if (newEntity == null)
                        newEntity = new NbtCompound(new[] { entitySelf });
                    else
                        newEntity["Items"] = entitySelf;
                    world.SetTileEntity(self, newEntity);
                    if (adjacent != -Coordinates3D.One)
                    {
                        newEntity = world.GetTileEntity(adjacent);
                        if (newEntity == null)
                            newEntity = new NbtCompound(new[] { entityAdjacent });
                        else
                            newEntity["Items"] = entityAdjacent;
                        world.SetTileEntity(adjacent, newEntity);
                    }
                };
            user.OpenWindow(window);
            return false;
        }

        public override void BlockMined(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            var self = descriptor.Coordinates;
            var entity = world.GetTileEntity(self);
            var manager = user.Server.GetEntityManagerForWorld(world);
            if (entity != null)
            {
                foreach (var item in (NbtList)entity["Items"])
                {
                    var slot = ItemStack.FromNbt((NbtCompound)item);
                    manager.SpawnEntity(new ItemEntity(descriptor.Coordinates + new Vector3(0.5), slot));
                }
            }
            world.SetTileEntity(self, null);
            base.BlockMined(descriptor, face, world, user);
        }
    }
}
