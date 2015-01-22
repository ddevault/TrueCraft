using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class PoweredRailBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x1B;
        
        public override byte ID { get { return 0x1B; } }
        
        public override double BlastResistance { get { return 3.5; } }

        public override double Hardness { get { return 0.7; } }

        public override byte Luminance { get { return 0; } }
        
        public override string DisplayName { get { return "Powered Rail"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(3, 11);
        }
    }
}