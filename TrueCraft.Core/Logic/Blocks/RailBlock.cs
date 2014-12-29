using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class RailBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x42;
        
        public override byte ID { get { return 0x42; } }

        public override double Hardness { get { return 0.7; } }

        public override string DisplayName { get { return "Rail"; } }
    }
}