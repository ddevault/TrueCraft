using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class SignBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x44;
        
        public override byte ID { get { return 0x44; } }

        public override double Hardness { get { return 1; } }

        public override string DisplayName { get { return "Sign"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(4, 0);
        }
    }
}