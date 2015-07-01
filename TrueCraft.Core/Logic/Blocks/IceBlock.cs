using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.API.World;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Logic.Blocks
{
    public class IceBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x4F;
        
        public override byte ID { get { return 0x4F; } }
        
        public override double BlastResistance { get { return 2.5; } }

        public override double Hardness { get { return 0.5; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }

        public override byte LightOpacity { get { return 2; } }
        
        public override string DisplayName { get { return "Ice"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(3, 4);
        }

        public override void BlockMined(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            world.SetBlockID(descriptor.Coordinates, WaterBlock.BlockID);
        }
    }
}