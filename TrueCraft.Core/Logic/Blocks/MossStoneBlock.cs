using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class MossStoneBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x30;
        
        public override byte ID { get { return 0x30; } }

        public override double Hardness { get { return 2; } }

        public override string DisplayName { get { return "Moss Stone"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(4, 2);
        }
    }
}