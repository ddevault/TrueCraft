using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class CactusBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x51;
        
        public override byte ID { get { return 0x51; } }

        public override double Hardness { get { return 0.4; } }

        public override string DisplayName { get { return "Cactus"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(6, 4);
        }
    }
}