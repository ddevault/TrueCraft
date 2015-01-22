using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class RedstoneOreBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x49;

        public override byte ID { get { return 0x49; } }

        public override double BlastResistance { get { return 15; } }

        public override double Hardness { get { return 3; } }

        public override byte Luminance { get { return 0; } }

        public override string DisplayName { get { return "Redstone Ore"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(3, 3);
        }
    }

    public class GlowingRedstoneOreBlock : RedstoneOreBlock
    {
        public static readonly new byte BlockID = 0x4A;
        
        public override byte ID { get { return 0x4A; } }

        public override byte Luminance { get { return 9; } }
        
        public override string DisplayName { get { return "Redstone Ore (glowing)"; } }
    }
}