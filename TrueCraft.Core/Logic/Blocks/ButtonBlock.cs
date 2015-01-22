using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class ButtonBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x4D;
        
        public override byte ID { get { return 0x4D; } }
        
        public override double BlastResistance { get { return 2.5; } }

        public override double Hardness { get { return 0.5; } }

        public override byte Luminance { get { return 0; } }
        
        public override string DisplayName { get { return "Button"; } }
    }
}