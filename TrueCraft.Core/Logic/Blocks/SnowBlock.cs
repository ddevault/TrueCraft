using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class SnowBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x50;
        
        public override byte ID { get { return 0x50; } }

        public override double Hardness { get { return 0.2; } }

        public override string DisplayName { get { return "Snow Block"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(2, 4);
        }
    }

    public class SnowfallBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x4E;

        public override byte ID { get { return 0x4E; } }

        public override double Hardness { get { return 0.1; } }

        public override string DisplayName { get { return "Snow"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(3, 4);
        }
    }
}