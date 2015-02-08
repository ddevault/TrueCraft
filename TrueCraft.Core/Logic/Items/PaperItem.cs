using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class PaperItem : ItemProvider, ICraftingRecipe
    {
        public static readonly short ItemID = 0x153;

        public override short ID { get { return 0x153; } }

        public override string DisplayName { get { return "Paper"; } }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { new ItemStack(SugarCanesItem.ItemID), new ItemStack(SugarCanesItem.ItemID), new ItemStack(SugarCanesItem.ItemID) },
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