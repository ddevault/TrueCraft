using System;
using TrueCraft.API;
using TrueCraft.API.Logic;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks
{
    public class BookshelfBlock : BlockProvider, ICraftingRecipe
    {
        public static readonly byte BlockID = 0x2F;
        
        public override byte ID { get { return 0x2F; } }
        
        public override double BlastResistance { get { return 7.5; } }

        public override double Hardness { get { return 1.5; } }

        public override byte Luminance { get { return 0; } }
        
        public override string DisplayName { get { return "Bookshelf"; } }

        public override SoundEffectClass SoundEffect
        {
            get
            {
                return SoundEffectClass.Wood;
            }
        }

        public override bool Flammable { get { return true; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(3, 2);
        }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    {
                        new ItemStack(WoodenPlanksBlock.BlockID),
                        new ItemStack(WoodenPlanksBlock.BlockID),
                        new ItemStack(WoodenPlanksBlock.BlockID)
                    },
                    {
                        new ItemStack(BookItem.ItemID),
                        new ItemStack(BookItem.ItemID),
                        new ItemStack(BookItem.ItemID)
                    },
                    {
                        new ItemStack(WoodenPlanksBlock.BlockID),
                        new ItemStack(WoodenPlanksBlock.BlockID),
                        new ItemStack(WoodenPlanksBlock.BlockID)
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