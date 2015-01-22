using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class LavaBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x0A;

        public override byte ID { get { return 0x0A; } }

        public override double BlastResistance { get { return 0; } }

        public override double Hardness { get { return 100; } }

        public override byte Luminance { get { return 15; } }

        public override string DisplayName { get { return "Lava"; } }
    }

    public class StationaryLavaBlock : LavaBlock
    {
        public static readonly new byte BlockID = 0x0B;
        
        public override byte ID { get { return 0x0B; } }
        
        public override double BlastResistance { get { return 500; } }

        public override string DisplayName { get { return "Lava (stationary)"; } }
    }
}