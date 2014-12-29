using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class CoalOreBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x10;
        
        public override byte ID { get { return 0x10; } }

        public override double Hardness { get { return 3; } }

        public override string DisplayName { get { return "Coal Ore"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(2, 2);
        }
    }
}