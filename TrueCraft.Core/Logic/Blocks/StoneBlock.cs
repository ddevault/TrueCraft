using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class StoneBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x01;
        
        public override byte ID { get { return 0x01; } }

        public override double Hardness { get { return 1.5; } }

        public override string DisplayName { get { return "Stone"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(1, 0);
        }
    }
}