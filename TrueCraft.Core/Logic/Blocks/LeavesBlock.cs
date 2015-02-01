using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.API.World;

namespace TrueCraft.Core.Logic.Blocks
{
    public class LeavesBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x12;
        
        public override byte ID { get { return 0x12; } }
        
        public override double BlastResistance { get { return 1; } }

        public override double Hardness { get { return 0.2; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }

        //TODO: Mark this as a block that diffuses sun light.
        
        public override string DisplayName { get { return "Leaves"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(4, 3);
        }

        protected override ItemStack[] GetDrop(BlockDescriptor descriptor)
        {
            if (MathHelper.Random.Next(20) == 0) // 5% chance
                return new[] { new ItemStack(SaplingBlock.BlockID, 1, descriptor.Metadata) };
            else
                return new ItemStack[0];
        }
    }
}