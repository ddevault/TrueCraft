using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class LavaBlock : FluidBlock
    {
        public LavaBlock() : this(false)
        {
        }

        public LavaBlock(bool nether)
        {
            if (nether)
                _MaximumFluidDepletion = 7;
            else
                _MaximumFluidDepletion = 3;
        }

        public static readonly byte BlockID = 0x0A;

        public override byte ID { get { return 0x0A; } }

        public override double BlastResistance { get { return 0; } }

        public override double Hardness { get { return 100; } }

        public override byte Luminance { get { return 15; } }

        public override bool Opaque { get { return false; } }

        public override byte LightOpacity { get { return 255; } }

        public override string DisplayName { get { return "Lava"; } }

        protected override bool AllowSourceCreation { get { return false; } }

        protected override double SecondsBetweenUpdates { get { return 2; } }

        private byte _MaximumFluidDepletion { get; set; }
        protected override byte MaximumFluidDepletion { get { return _MaximumFluidDepletion; } }

        protected override byte FlowingID { get { return BlockID; } }

        protected override byte StillID { get { return StationaryLavaBlock.BlockID; } }
    }

    public class StationaryLavaBlock : LavaBlock
    {
        public static readonly new byte BlockID = 0x0B;
        
        public override byte ID { get { return 0x0B; } }
        
        public override double BlastResistance { get { return 500; } }

        public override string DisplayName { get { return "Lava (stationary)"; } }
    }
}