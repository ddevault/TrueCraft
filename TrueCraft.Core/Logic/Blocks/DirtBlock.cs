using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class DirtBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x03;
        
        public override byte ID { get { return 0x03; } }
        
        public override double BlastResistance { get { return 2.5; } }

        public override double Hardness { get { return 0.5; } }

        public override byte Luminance { get { return 0; } }
        
        public override string DisplayName { get { return "Dirt"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(2, 0);
        }
    }
}