using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class DetectorRailBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x1C;
        
        public override byte ID { get { return 0x1C; } }

        public override double Hardness { get { return 0.7; } }

        public override string DisplayName { get { return "Detector Rail"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(3, 12);
        }
    }
}