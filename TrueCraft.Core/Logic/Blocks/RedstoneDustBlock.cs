using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class RedstoneDustBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x37;
        
        public override byte ID { get { return 0x37; } }

        public override double Hardness { get { return 0; } }

        public override string DisplayName { get { return "Redstone Dust"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(4, 10);
        }
    }
}