using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class StoneSlabBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x2C;
        
        public override byte ID { get { return 0x2C; } }

        public override double Hardness { get { return 2; } }

        public override string DisplayName { get { return "Stone Slab"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(6, 0);
        }
    }

    public class DoubleStoneSlabBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x2B;

        public override byte ID { get { return 0x2B; } }

        public override double Hardness { get { return 2; } }

        public override string DisplayName { get { return "Double Stone Slab"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(6, 0);
        }
    }
}