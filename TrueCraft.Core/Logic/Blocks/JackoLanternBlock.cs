using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class JackoLanternBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x5B;
        
        public override byte ID { get { return 0x5B; } }
        
        public override double BlastResistance { get { return 5; } }

        public override double Hardness { get { return 1; } }

        public override byte Luminance { get { return 15; } }

        public override bool Opaque { get { return false; } }

        public override byte LightModifier { get { return 255; } }
        
        public override string DisplayName { get { return "Jack 'o' Lantern"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(6, 6);
        }
    }
}