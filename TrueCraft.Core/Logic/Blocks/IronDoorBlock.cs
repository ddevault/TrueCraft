using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class IronDoorBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x47;
        
        public override byte ID { get { return 0x47; } }
        
        public override double BlastResistance { get { return 25; } }

        public override double Hardness { get { return 5; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }
        
        public override string DisplayName { get { return "Iron Door"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(1, 6);
        }
    }
}