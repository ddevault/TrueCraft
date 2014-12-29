using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class IronBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x2A;
        
        public override byte ID { get { return 0x2A; } }

        public override double Hardness { get { return 5; } }

        public override string DisplayName { get { return "Block of Iron"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(6, 1);
        }
    }
}