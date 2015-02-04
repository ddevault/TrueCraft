using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks
{
    public class GravelBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x0D;
        
        public override byte ID { get { return 0x0D; } }
        
        public override double BlastResistance { get { return 3; } }

        public override double Hardness { get { return 0.6; } }

        public override byte Luminance { get { return 0; } }
        
        public override string DisplayName { get { return "Gravel"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(3, 1);
        }

        protected override ItemStack[] GetDrop(BlockDescriptor descriptor)
        {
            //Gravel has a 10% chance of dropping flint.
            if (MathHelper.Random.Next(10) == 0)
                return new[] { new ItemStack(FlintItem.ItemID, 1, descriptor.Metadata) };
            else
                return new ItemStack[0];
        }
    }
}