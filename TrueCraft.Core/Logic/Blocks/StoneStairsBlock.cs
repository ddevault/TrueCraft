using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class StoneStairsBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x43;
        
        public override byte ID { get { return 0x43; } }

        public override double Hardness { get { return 0; } }

        public override string DisplayName { get { return "Stone Stairs"; } }
    }
}