using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class PistonBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x21;
        
        public override byte ID { get { return 0x21; } }
        
        public override double BlastResistance { get { return 2.5; } }

        public override double Hardness { get { return 0.5; } }

        public override byte Luminance { get { return 0; } }
        
        public override string DisplayName { get { return "Piston"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(11, 6);
        }
    }

    public class StickyPistonBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x1D;

        public override byte ID { get { return 0x1D; } }

        public override double BlastResistance { get { return 2.5; } }

        public override double Hardness { get { return 0.5; } }

        public override byte Luminance { get { return 0; } }

        public override string DisplayName { get { return "Sticky Piston"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(10, 6);
        }
    }

    public class PistonPlungerBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x22;

        public override byte ID { get { return 0x22; } }

        public override double BlastResistance { get { return 2.5; } }

        public override double Hardness { get { return 0.5; } }

        public override byte Luminance { get { return 0; } }

        public override string DisplayName { get { return "Piston Plunger"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(11, 6);
        }
    }

    public class PistonPlaceholderBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x24;

        public override byte ID { get { return 0x24; } }

        public override double BlastResistance { get { return 0; } }

        public override double Hardness { get { return 0; } }

        public override byte Luminance { get { return 0; } }

        public override string DisplayName { get { return "Piston Placeholder"; } }
    }
}