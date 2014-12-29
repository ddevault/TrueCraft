using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class BedrockBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x07;
        
        public override byte ID { get { return 0x07; } }

        public override double Hardness { get { return 0; } }

        public override string DisplayName { get { return "Bedrock"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(1, 1);
        }
    }
}