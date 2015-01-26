using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public abstract class PressurePlateBlock : BlockProvider
    {
        public override double BlastResistance { get { return 2.5; } }

        public override double Hardness { get { return 0.5; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }
    }

    public class WoodenPressurePlateBlock : PressurePlateBlock
    {
        public static readonly byte BlockID = 0x48;
        
        public override byte ID { get { return 0x48; } }
        
        public override string DisplayName { get { return "Wooden Pressure Plate"; } }
    }

    public class StonePressurePlateBlock : PressurePlateBlock {
        public static readonly byte BlockID = 0x46;

        public override byte ID { get { return 0x46; } }

        public override string DisplayName { get { return "Stone Pressure Plate"; } }
    }
}