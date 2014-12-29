using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class DiamondOreBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x38;
        
        public override byte ID { get { return 0x38; } }

        public override double Hardness { get { return 3; } }

        public override string DisplayName { get { return "Diamond Ore"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(2, 3);
        }
    }
}