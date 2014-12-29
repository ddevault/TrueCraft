using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class JackoLanternBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x5B;
        
        public override byte ID { get { return 0x5B; } }

        public override double Hardness { get { return 1; } }

        public override string DisplayName { get { return "Jack 'o' Lantern"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(6, 6);
        }
    }
}