using System;
using TrueCraft.API;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class AirBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x00;
        
        public override byte ID { get { return 0x00; } }

        public override double BlastResistance { get { return 0; } }

        public override double Hardness { get { return 0; } }

        public override bool Opaque { get { return false; } }

        public override byte Luminance { get { return 0; } }

        public override string DisplayName { get { return "Air"; } }

        public override BoundingBox? BoundingBox { get { return null; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(0, 0);
        }

        protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
        {
            return new ItemStack[0];
        }
    }
}
