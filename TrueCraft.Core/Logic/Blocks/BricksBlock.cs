using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class BricksBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x2D;
        
        public override byte ID { get { return 0x2D; } }

        public override double Hardness { get { return 2; } }

        public override string DisplayName { get { return "Bricks"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(7, 0);
        }
    }
}