using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks
{
    public class GlowstoneBlock : BlockProvider, ICraftingRecipe
    {
        public static readonly byte BlockID = 0x59;
        
        public override byte ID { get { return 0x59; } }
        
        public override double BlastResistance { get { return 1.5; } }

        public override double Hardness { get { return 0.3; } }

        public override byte Luminance { get { return 15; } }

        public override bool Opaque { get { return false; } }
        
        public override string DisplayName { get { return "Glowstone"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(9, 6);
        }

        protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
        {
            return new[] { new ItemStack(GlowstoneDustItem.ItemID, (sbyte)new Random().Next(2, 4), descriptor.Metadata) };
        }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    {
                        new ItemStack(GlowstoneDustItem.ItemID),
                        new ItemStack(GlowstoneDustItem.ItemID)
                    },
                    {
                        new ItemStack(GlowstoneDustItem.ItemID),
                        new ItemStack(GlowstoneDustItem.ItemID)
                    }
                };
            }
        }

        public ItemStack Output
        {
            get { return new ItemStack(BlockID); }
        }

        public bool SignificantMetadata
        {
            get { return false; }
        }
    }
}