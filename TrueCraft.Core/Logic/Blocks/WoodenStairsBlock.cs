using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class WoodenStairsBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x35;
        
        public override byte ID { get { return 0x35; } }

        public override double Hardness { get { return 0; } }

        public override string DisplayName { get { return "Wooden Stairs"; } }
    }
}