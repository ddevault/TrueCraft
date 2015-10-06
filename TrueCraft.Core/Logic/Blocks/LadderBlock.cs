using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.Core.Logic.Items;
using TrueCraft.API.World;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Logic.Blocks
{
    public class LadderBlock : BlockProvider, ICraftingRecipe
    {
        /// <summary>
        /// The side of the block that this ladder is attached to (i.e. "the north side")
        /// </summary>
        public enum LadderDirection
        {
            East = 0x04,
            West = 0x05,
            North = 0x03,
            South = 0x02
        }

        public static readonly byte BlockID = 0x41;
        
        public override byte ID { get { return 0x41; } }
        
        public override double BlastResistance { get { return 2; } }

        public override double Hardness { get { return 0.4; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }
        
        public override string DisplayName { get { return "Ladder"; } }

        public override SoundEffectClass SoundEffect
        {
            get
            {
                return SoundEffectClass.Wood;
            }
        }

        public override BoundingBox? BoundingBox { get { return null; } }

        public override BoundingBox? InteractiveBoundingBox
        {
            get
            {
                return new BoundingBox(new Vector3(0.25, 0, 0.25), new Vector3(0.75, 0.5, 0.75));
            }
        }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(3, 5);
        }

        public override Coordinates3D GetSupportDirection(BlockDescriptor descriptor)
        {
            switch ((LadderDirection)descriptor.Metadata)
            {
                case LadderDirection.East:
                    return Coordinates3D.East;
                case LadderDirection.West:
                    return Coordinates3D.West;
                case LadderDirection.North:
                    return Coordinates3D.North;
                case LadderDirection.South:
                    return Coordinates3D.South;
                default:
                    return Coordinates3D.Zero;
            }
        }

        public override void ItemUsedOnBlock(Coordinates3D coordinates, ItemStack item, BlockFace face, IWorld world, IRemoteClient user)
        {
            coordinates += MathHelper.BlockFaceToCoordinates(face);
            var descriptor = world.GetBlockData(coordinates);
            LadderDirection direction;
            switch (MathHelper.DirectionByRotationFlat(user.Entity.Yaw))
            {
                case Direction.North:
                    direction = LadderDirection.North;
                    break;
                case Direction.South:
                    direction = LadderDirection.South;
                    break;
                case Direction.East:
                    direction = LadderDirection.East;
                    break;
                default:
                    direction = LadderDirection.West;
                    break;
            }
            descriptor.Metadata = (byte)direction;
            if (IsSupported(descriptor, user.Server, world))
            {
                world.SetBlockID(descriptor.Coordinates, BlockID);
                world.SetMetadata(descriptor.Coordinates, (byte)direction);
                item.Count--;
                user.Inventory[user.SelectedSlot] = item;
            }
        }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    {
                        new ItemStack(StickItem.ItemID),
                        ItemStack.EmptyStack,
                        new ItemStack(StickItem.ItemID)
                    },
                    {
                        new ItemStack(StickItem.ItemID),
                        new ItemStack(StickItem.ItemID),
                        new ItemStack(StickItem.ItemID)
                    },
                    {
                        new ItemStack(StickItem.ItemID),
                        ItemStack.EmptyStack,
                        new ItemStack(StickItem.ItemID)
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
    }
}