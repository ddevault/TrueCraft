using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class PumpkinBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x56;
        
        public override byte ID { get { return 0x56; } }

        public override double Hardness { get { return 1; } }

        public override string DisplayName { get { return "Pumpkin"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(6, 6);
        }
    }
}