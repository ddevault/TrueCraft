using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class WaterBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x08;
        
        public override byte ID { get { return 0x08; } }

        public override double Hardness { get { return 100; } }

        public override string DisplayName { get { return "Water"; } }
    }

    public class StationaryWaterBlock : WaterBlock
    {
        public static readonly new byte BlockID = 0x09;

        public override byte ID { get { return 0x09; } }

        public override string DisplayName { get { return "Water (stationary)"; } }
    }
}