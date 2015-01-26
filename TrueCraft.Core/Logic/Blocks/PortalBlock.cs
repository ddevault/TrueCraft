using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class PortalBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x5A;
        
        public override byte ID { get { return 0x5A; } }
        
        public override double BlastResistance { get { return 0; } }

        public override double Hardness { get { return -1; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }
        
        public override string DisplayName { get { return "Portal"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(14, 0);
        }
    }
}