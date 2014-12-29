using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class WoolBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x23;
        
        public override byte ID { get { return 0x23; } }

        public override double Hardness { get { return 0.8; } }

        public override string DisplayName { get { return "Wool"; } }
    }
}