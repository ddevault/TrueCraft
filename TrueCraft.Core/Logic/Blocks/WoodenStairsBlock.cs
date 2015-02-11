using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.API.World;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Logic.Blocks
{
    public class WoodenStairsBlock : BlockProvider, ICraftingRecipe
    {
        public enum StairDirection
        {
            East = 0,
            West = 1,
            South = 2,
            North = 3
        }

        public static readonly byte BlockID = 0x35;
        
        public override byte ID { get { return 0x35; } }
        
        public override double BlastResistance { get { return 15; } }

        public override double Hardness { get { return 0; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }

        public override byte LightModifier { get { return 255; } }
        
        public override string DisplayName { get { return "Wooden Stairs"; } }

        public virtual ItemStack[,] Pattern
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

        public virtual ItemStack Output
        {
            get
            {
                return new ItemStack(BlockID);
            }
        }

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
}