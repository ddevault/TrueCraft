using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.API.World;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Logic.Blocks
{
    public abstract class StairsBlock : BlockProvider
    {
        public enum StairDirection
        {
            East = 0,
            West = 1,
            South = 2,
            North = 3
        }

        public override double Hardness { get { return 0; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }

        public override byte LightOpacity { get { return 255; } }

        public virtual bool SignificantMetadata
        {
            get
            {
                return false;
            }
        }

        public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            byte meta = 0;
            switch (MathHelper.DirectionByRotationFlat(user.Entity.Yaw))
            {
                case Direction.East:
                    meta = (byte)StairDirection.East;
                    break;
                case Direction.West:
                    meta = (byte)StairDirection.West;
                    break;
                case Direction.North:
                    meta = (byte)StairDirection.North;
                    break;
                case Direction.South:
                    meta = (byte)StairDirection.South;
                    break;
                default:
                    meta = 0; // Should never happen
                    break;
            }
            world.SetMetadata(descriptor.Coordinates, meta);
        }
    }

    public class WoodenStairsBlock : StairsBlock, ICraftingRecipe
    {
        public static readonly byte BlockID = 0x35;
        
        public override byte ID { get { return 0x35; } }
        
        public override double BlastResistance { get { return 15; } }
        
        public override string DisplayName { get { return "Wooden Stairs"; } }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { new ItemStack(WoodenPlanksBlock.BlockID), ItemStack.EmptyStack, ItemStack.EmptyStack },
                    { new ItemStack(WoodenPlanksBlock.BlockID), new ItemStack(WoodenPlanksBlock.BlockID), ItemStack.EmptyStack },
                    { new ItemStack(WoodenPlanksBlock.BlockID), new ItemStack(WoodenPlanksBlock.BlockID), new ItemStack(WoodenPlanksBlock.BlockID) }
                };
            }
        }

        public ItemStack Output
        {
            get
            {
                return new ItemStack(BlockID);
            }
        }
    }

    public class StoneStairsBlock : StairsBlock, ICraftingRecipe
    {
        public static readonly byte BlockID = 0x43;

        public override byte ID { get { return 0x43; } }

        public override double BlastResistance { get { return 30; } }

        public override string DisplayName { get { return "Stone Stairs"; } }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { new ItemStack(StoneBlock.BlockID), ItemStack.EmptyStack, ItemStack.EmptyStack },
                    { new ItemStack(StoneBlock.BlockID), new ItemStack(StoneBlock.BlockID), ItemStack.EmptyStack },
                    { new ItemStack(StoneBlock.BlockID), new ItemStack(StoneBlock.BlockID), new ItemStack(StoneBlock.BlockID) }
                };
            }
        }

        public ItemStack Output
        {
            get
            {
                return new ItemStack(BlockID);
            }
        }
    }
}