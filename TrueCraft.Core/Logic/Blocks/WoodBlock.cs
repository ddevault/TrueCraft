using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class WoodBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x11;
        
        public override byte ID { get { return 0x11; } }

        public override double Hardness { get { return 2; } }

        public override string DisplayName { get { return "Wood"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(4, 1);
        }
    }
}