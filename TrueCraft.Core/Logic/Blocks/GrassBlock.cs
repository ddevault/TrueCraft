using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.API.World;

namespace TrueCraft.Core.Logic.Blocks
{
    public class GrassBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x02;
        
        public override byte ID { get { return 0x02; } }
        
        public override double BlastResistance { get { return 3; } }

        public override double Hardness { get { return 0.6; } }

        public override byte Luminance { get { return 0; } }

        public override string DisplayName { get { return "Grass"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(0, 0); // TODO: Figure out how to handle more complex situations
        }

        protected override ItemStack[] GetDrop(BlockDescriptor descriptor)
        {
            return new[] { new ItemStack(DirtBlock.BlockID, 1) };
        }
    }
}