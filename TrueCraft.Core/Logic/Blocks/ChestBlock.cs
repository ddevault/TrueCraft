using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class ChestBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x36;
        
        public override byte ID { get { return 0x36; } }

        public override double Hardness { get { return 2.5; } }

        public override string DisplayName { get { return "Chest"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(10, 1);
        }
    }
}