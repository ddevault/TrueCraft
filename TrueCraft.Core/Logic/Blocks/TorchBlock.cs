using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class TorchBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x32;
        
        public override byte ID { get { return 0x32; } }

        public override double Hardness { get { return 0; } }

        public override string DisplayName { get { return "Torch"; } }
    }
}