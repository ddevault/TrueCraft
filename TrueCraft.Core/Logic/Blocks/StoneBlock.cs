using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Blocks
{
    public class StoneBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x01;
        
        public override byte ID { get { return 0x01; } }
        
        public override double BlastResistance { get { return 30; } }

        public override double Hardness { get { return 1.5; } }

        public override byte Luminance { get { return 0; } }
        
        public override string DisplayName { get { return "Stone"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(1, 0);
        }

        protected override ItemStack[] GetDrop(BlockDescriptor descriptor)
        {
            return new[] { new ItemStack(CobblestoneBlock.BlockID, 1, descriptor.Metadata) };
        }
    }
}