using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class RoseBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x26;
        
        public override byte ID { get { return 0x26; } }
        
        public override double BlastResistance { get { return 0; } }

        public override double Hardness { get { return 0; } }

        public override byte Luminance { get { return 0; } }
        
        public override string DisplayName { get { return "Rose"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(12, 0);
        }
    }
}