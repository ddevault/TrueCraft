using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.API.World;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Logic.Blocks
{
    public class PumpkinBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x56;
        
        public override byte ID { get { return 0x56; } }
        
        public override double BlastResistance { get { return 5; } }

        public override double Hardness { get { return 1; } }

        public override byte Luminance { get { return 0; } }
        
        public override string DisplayName { get { return "Pumpkin"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(6, 6);
        }

        public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            world.SetMetadata(descriptor.Coordinates, (byte)MathHelper.DirectionByRotationFlat(user.Entity.Yaw, true));
        }
    }
}