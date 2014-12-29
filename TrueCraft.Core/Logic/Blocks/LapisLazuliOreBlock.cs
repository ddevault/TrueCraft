using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class LapisLazuliOreBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x15;
        
        public override byte ID { get { return 0x15; } }

        public override double Hardness { get { return 3; } }

        public override string DisplayName { get { return "Lapis Lazuli Ore"; } }
    }
}