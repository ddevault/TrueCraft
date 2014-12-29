using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class RedstoneTorchBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x4C;
        
        public override byte ID { get { return 0x4C; } }

        public override double Hardness { get { return 0; } }

        public override string DisplayName { get { return "Redstone Torch"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(3, 7);
        }
    }

    public class InactiveRedstoneTorchBlock : RedstoneTorchBlock
    {
        public static readonly new byte BlockID = 0x4B;

        public override byte ID { get { return 0x4B; } }

        public override string DisplayName { get { return "Redstone Torch (inactive)"; } }
    }
}