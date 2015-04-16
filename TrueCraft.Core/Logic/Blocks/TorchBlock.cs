using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.Core.Logic.Items;
using TrueCraft.API.World;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Logic.Blocks
{
    public class TorchBlock : BlockProvider, ICraftingRecipe
    {
        public enum TorchDirection
        {
            South = 0x01, // Positive Z
            North = 0x02,
            West = 0x03,
            East = 0x04,
            Ground = 0x05
        }

        public static readonly byte BlockID = 0x32;
        
        public override byte ID { get { return 0x32; } }
        
        public override double BlastResistance { get { return 0; } }

        public override double Hardness { get { return 0; } }

        public override byte Luminance { get { return 13; } }

        public override bool Opaque { get { return false; } }
        
        public override string DisplayName { get { return "Torch"; } }

        public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            TorchDirection direction;
            switch (face)
            {
                case BlockFace.PositiveZ:
                    direction = TorchDirection.West;
                    break;
                case BlockFace.NegativeZ:
                    direction = TorchDirection.East;
                    break;
                case BlockFace.PositiveX:
                    direction = TorchDirection.South;
                    break;
                case BlockFace.NegativeX:
                    direction = TorchDirection.North;
                    break;
                default:
                    direction = TorchDirection.Ground;
                    break;
            }
            world.SetMetadata(descriptor.Coordinates, (byte)direction);
        }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(0, 5);
        }
            
        public virtual ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { new ItemStack(CoalItem.ItemID) },
                    { new ItemStack(StickItem.ItemID) }
                };
            }
        }

        public virtual ItemStack Output
        {
            get
            {
                return new ItemStack(TorchBlock.BlockID, 4);
            }
        }

        public virtual bool SignificantMetadata
        {
            get
            {
                return false;
            }
        }
    }
}