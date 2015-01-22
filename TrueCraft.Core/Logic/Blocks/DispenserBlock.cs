using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class DispenserBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x17;
        
        public override byte ID { get { return 0x17; } }
        
        public override double BlastResistance { get { return 17.5; } }

        public override double Hardness { get { return 3.5; } }

        public override byte Luminance { get { return 0; } }
        
        public override string DisplayName { get { return "Dispenser"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(13, 2);
        }
    }
}