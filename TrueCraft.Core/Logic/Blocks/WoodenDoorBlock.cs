using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class WoodenDoorBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x40;
        
        public override byte ID { get { return 0x40; } }

        public override double Hardness { get { return 3; } }

        public override string DisplayName { get { return "Wooden Door"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(1, 6);
        }
    }
}