using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class GoldOreBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x0E;
        
        public override byte ID { get { return 0x0E; } }

        public override double Hardness { get { return 3; } }

        public override string DisplayName { get { return "Gold Ore"; } }
    }
}