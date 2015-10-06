using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks
{
    public class FenceBlock : BlockProvider, ICraftingRecipe
    {
        public static readonly byte BlockID = 0x55;
        
        public override byte ID { get { return 0x55; } }
        
        public override double BlastResistance { get { return 15; } }

        public override double Hardness { get { return 2; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }

        public override bool Flammable { get { return true; } }
        
        public override string DisplayName { get { return "Fence"; } }

        public override SoundEffectClass SoundEffect
        {
            get
            {
                return SoundEffectClass.Wood;
            }
        }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(4, 0);
        }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    {
                        new ItemStack(StickItem.ItemID),
                        new ItemStack(WoodenPlanksBlock.BlockID),
                        new ItemStack(StickItem.ItemID)
                    },
                    {
                        new ItemStack(StickItem.ItemID),
                        new ItemStack(WoodenPlanksBlock.BlockID),
                        new ItemStack(StickItem.ItemID)
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