using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class TorchBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x32;
        
        public override byte ID { get { return 0x32; } }
        
        public override double BlastResistance { get { return 0; } }

        public override double Hardness { get { return 0; } }

        public override byte Luminance { get { return 13; } }

        public override bool Opaque { get { return false; } }
        
        public override string DisplayName { get { return "Torch"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(0, 5);
        }
    }
}