using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class NetherrackBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x57;
        
        public override byte ID { get { return 0x57; } }
        
        public override double BlastResistance { get { return 2; } }

        public override double Hardness { get { return 0.4; } }

        public override byte Luminance { get { return 0; } }
        
        public override string DisplayName { get { return "Netherrack"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(7, 6);
        }
    }
}