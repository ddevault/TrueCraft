using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class GrassBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x02;
        
        public override byte ID { get { return 0x02; } }
        
        public override double BlastResistance { get { return 3; } }

        public override double Hardness { get { return 0.6; } }

        public override byte Luminance { get { return 0; } }
        
        public override string DisplayName { get { return "Grass"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(3, 0);
        }
    }
}