using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class BedBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x1A;
        
        public override byte ID { get { return 0x1A; } }

        public override double Hardness { get { return 0.2; } }

        public override string DisplayName { get { return "Bed"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(6, 8);
        }
    }
}