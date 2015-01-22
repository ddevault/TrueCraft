using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class SaplingBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x06;
        
        public override byte ID { get { return 0x06; } }
        
        public override double BlastResistance { get { return 0; } }

        public override double Hardness { get { return 0; } }

        public override byte Luminance { get { return 0; } }
        
        public override string DisplayName { get { return "Sapling"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(15, 0);
        }
    }
}