using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class GravelBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x0D;
        
        public override byte ID { get { return 0x0D; } }

        public override double Hardness { get { return 0.6; } }

        public override string DisplayName { get { return "Gravel"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(3, 1);
        }
    }
}