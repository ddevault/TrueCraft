using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Blocks
{
    public class GlassBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x14;
        
        public override byte ID { get { return 0x14; } }
        
        public override double BlastResistance { get { return 1.5; } }

        public override double Hardness { get { return 0.3; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }
        
        public override string DisplayName { get { return "Glass"; } }

        public override byte LightOpacity { get { return 0; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(1, 3);
        }

        protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
        {
            return new ItemStack[0];
        }
    }
}
