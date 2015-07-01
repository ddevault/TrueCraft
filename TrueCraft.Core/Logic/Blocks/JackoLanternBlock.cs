using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.API.World;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Logic.Blocks
{
    public class JackoLanternBlock : BlockProvider, ICraftingRecipe
    {
        public static readonly byte BlockID = 0x5B;
        
        public override byte ID { get { return 0x5B; } }
        
        public override double BlastResistance { get { return 5; } }

        public override double Hardness { get { return 1; } }

        public override byte Luminance { get { return 15; } }

        public override bool Opaque { get { return false; } }

        public override byte LightOpacity { get { return 255; } }
        
        public override string DisplayName { get { return "Jack 'o' Lantern"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(6, 6);
        }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { new ItemStack(PumpkinBlock.BlockID) },
                    { new ItemStack(TorchBlock.BlockID) }
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

        public bool SignificantMetadata
        {
            get
            {
                return false;
            }
        }

        public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            world.SetMetadata(descriptor.Coordinates, (byte)MathHelper.DirectionByRotationFlat(user.Entity.Yaw, true));
        }
    }
}