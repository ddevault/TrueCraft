using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class GlowstoneBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x59;
        
        public override byte ID { get { return 0x59; } }

        public override double Hardness { get { return 0.3; } }

        public override string DisplayName { get { return "Glowstone"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(9, 6);
        }
    }
}