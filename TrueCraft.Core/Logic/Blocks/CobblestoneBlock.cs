using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class CobblestoneBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x04;
        
        public override byte ID { get { return 0x04; } }
        
        public override double BlastResistance { get { return 30; } }

        public override double Hardness { get { return 2; } }

        public override byte Luminance { get { return 0; } }
        
        public override string DisplayName { get { return "Cobblestone"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(0, 1);
        }
    }
}