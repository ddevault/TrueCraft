using System;
using TrueCraft.API.Logic;
using TrueCraft.API.Server;
using TrueCraft.API.World;

namespace TrueCraft.Core.Logic.Blocks
{
    public class WaterBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x08;

        public override byte ID { get { return 0x08; } }
        
        public override double BlastResistance { get { return 500; } }

        public override double Hardness { get { return 100; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }

        public override byte LightModifier { get { return 3; } }
        
        public override string DisplayName { get { return "Water"; } }

        public override void BlockUpdate(BlockDescriptor descriptor, IMultiplayerServer server, IWorld world)
        {
            base.BlockUpdate(descriptor, server, world);
        }
    }

    public class StationaryWaterBlock : WaterBlock
    {
        public static readonly new byte BlockID = 0x09;

        public override byte ID { get { return 0x09; } }

        public override string DisplayName { get { return "Water (stationary)"; } }
    }
}