using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class GoldBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x29;
        
        public override byte ID { get { return 0x29; } }
        
        public override double BlastResistance { get { return 30; } }

        public override double Hardness { get { return 3; } }

        public override byte Luminance { get { return 0; } }
        
        public override string DisplayName { get { return "Block of Gold"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(7, 1);
        }
    }
}