using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class FireBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x33;
        
        public override byte ID { get { return 0x33; } }
        
        public override double BlastResistance { get { return 0; } }

        public override double Hardness { get { return 0; } }

        public override byte Luminance { get { return 15; } }
        
        public override string DisplayName { get { return "Fire"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(15, 1);
        }
    }
}