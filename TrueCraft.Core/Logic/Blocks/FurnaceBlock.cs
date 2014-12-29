using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class FurnaceBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x3D;
        
        public override byte ID { get { return 0x3D; } }

        public override double Hardness { get { return 3.5; } }

        public override string DisplayName { get { return "Furnace"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(13, 2);
        }
    }
}