using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class LapisLazuliBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x16;
        
        public override byte ID { get { return 0x16; } }

        public override double Hardness { get { return 3; } }

        public override string DisplayName { get { return "Lapis Lazuli Block"; } }
    }
}