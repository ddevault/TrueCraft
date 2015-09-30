using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.Logic.Items
{
    public class GoldenAppleItem : FoodItem, ICraftingRecipe
    {
        public static readonly short ItemID = 0x142;

        public override short ID { get { return 0x142; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(11, 0);
        }

        public override float Restores { get { return 10; } }

        public override string DisplayName { get { return "Golden Apple"; } }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { 
                        new ItemStack(GoldBlock.BlockID),
                        new ItemStack(GoldBlock.BlockID),
                        new ItemStack(GoldBlock.BlockID),
                    },
                    { 
                        new ItemStack(GoldBlock.BlockID),
                        new ItemStack(AppleItem.ItemID),
                        new ItemStack(GoldBlock.BlockID),
                    },
                    { 
                        new ItemStack(GoldBlock.BlockID),
                        new ItemStack(GoldBlock.BlockID),
                        new ItemStack(GoldBlock.BlockID),
                    }
                };
            }
        }

        public ItemStack Output
        {
            get
            {
                return new ItemStack(ItemID);
            }
        }

        public bool SignificantMetadata
        {
            get
            {
                return true;
            }
        }
    }
}