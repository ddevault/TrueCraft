using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class FarmlandBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x3C;
        
        public override byte ID { get { return 0x3C; } }

        public override double Hardness { get { return 0.6; } }

        public override string DisplayName { get { return "Farmland"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(7, 5);
        }
    }
}