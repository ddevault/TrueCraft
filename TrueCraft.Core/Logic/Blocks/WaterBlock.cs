using System;
using TrueCraft.API.Logic;
using TrueCraft.API.Server;
using TrueCraft.API.World;
using TrueCraft.API;
using TrueCraft.API.Networking;
using System.Collections.Generic;
using System.Linq;

namespace TrueCraft.Core.Logic.Blocks
{
    public class WaterBlock : FluidBlock
    {
        public static readonly byte BlockID = 0x08;

        public override byte ID { get { return 0x08; } }
        
        public override double BlastResistance { get { return 500; } }

        public override double Hardness { get { return 100; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }

        public override byte LightOpacity { get { return 2; } }
        
        public override string DisplayName { get { return "Water"; } }

        protected override double SecondsBetweenUpdates { get { return 0.25; } }

        protected override byte MaximumFluidDepletion { get { return 7; } }

        protected override byte FlowingID { get { return BlockID; } }

        protected override byte StillID { get { return StationaryWaterBlock.BlockID; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(13, 12);
        }
    }

    public class StationaryWaterBlock : WaterBlock
    {
        public static readonly new byte BlockID = 0x09;

        public override byte ID { get { return 0x09; } }

        public override string DisplayName { get { return "Water (stationary)"; } }
    }
}