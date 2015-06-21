using System;
using TrueCraft.API;
using TrueCraft.Core.Logic.Items;
using TrueCraft.API.Logic;
using TrueCraft.API.World;
using TrueCraft.API.Networking;
using fNbt;
using TrueCraft.Core.Windows;

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
            bool unobstructed = true, isDouble = false;
            Coordinates3D other = Coordinates3D.Down;
            for (int i = 0; i < AdjacentBlocks.Length; i++)
            {
                if (world.GetBlockID(descriptor.Coordinates + AdjacentBlocks[i]) == ChestBlock.BlockID)
                {
                    isDouble = true;
                    other = descriptor.Coordinates + AdjacentBlocks[i];
                    var _ = world.BlockRepository.GetBlockProvider(world.GetBlockID(
                        descriptor.Coordinates + AdjacentBlocks[i] + Coordinates3D.Up));
                    if (_.Opaque)
                        unobstructed = false;
                }
            }
            if (world.BlockRepository.GetBlockProvider(world.GetBlockID(descriptor.Coordinates + Coordinates3D.Up)).Opaque)
                unobstructed = false;
            if (!unobstructed)
                return false;
            var entity = world.GetTileEntity(descriptor.Coordinates);
            var window = new ChestWindow((InventoryWindow)user.Inventory, isDouble);
            if (entity != null)
            {
                foreach (NbtCompound item in (NbtList)entity["Items"])
                {
                    var stack = ItemStack.FromNbt(item);
                    window.ChestInventory[stack.Index] = stack;
                }
            }
            if (isDouble)
            {
                entity = world.GetTileEntity(other);
                if (entity != null)
                {
                    foreach (NbtCompound item in (NbtList)entity["Items"])
                    {
                        var stack = ItemStack.FromNbt(item);
                        window.ChestInventory[stack.Index] = stack;
                    }
                }
            }
            user.OpenWindow(window);
            window.WindowChange += (sender, e) =>
            {
                // TODO: Update tile entity
            }; // TODO: Memory leak here
            return false;
        }
    }
}