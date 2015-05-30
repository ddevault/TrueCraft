using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.API.Networking;
using TrueCraft.API.World;

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