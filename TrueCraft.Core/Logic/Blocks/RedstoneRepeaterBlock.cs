using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class RedstoneRepeaterBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x5D;

        public override byte ID { get { return 0x5D; } }
        
        public override double BlastResistance { get { return 0; } }

        public override double Hardness { get { return 0; } }

        public override byte Luminance { get { return 0; } }
        
        public override string DisplayName { get { return "Redstone Repeater"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(6, 0);
        }
    }

    public class ActiveRedstoneRepeaterBlock : RedstoneRepeaterBlock
    {
        public static readonly new byte BlockID = 0x5E;

        public override byte ID { get { return 0x5E; } }

        public override string DisplayName { get { return "Redstone Repeater (active)"; } }
    }
}