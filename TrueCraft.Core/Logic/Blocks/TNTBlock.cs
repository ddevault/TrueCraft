using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class TNTBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x2E;
        
        public override byte ID { get { return 0x2E; } }
        
        public override double BlastResistance { get { return 0; } }

        public override double Hardness { get { return 0; } }

        public override byte Luminance { get { return 0; } }
        
        public override string DisplayName { get { return "TNT"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(8, 0);
        }
    }
}