using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class SpongeBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x13;
        
        public override byte ID { get { return 0x13; } }

        public override double Hardness { get { return 0.6; } }

        public override string DisplayName { get { return "Sponge"; } }
    }
}