using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class DeadBushBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x20;
        
        public override byte ID { get { return 0x20; } }

        public override double Hardness { get { return 0; } }

        public override string DisplayName { get { return "Dead Bush"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(7, 3);
        }
    }
}