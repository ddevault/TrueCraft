using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class BreadItem : FoodItem, ICraftingRecipe
    {
        public static readonly short ItemID = 0x129;

        public override short ID { get { return 0x129; } }

        public override float Restores { get { return 2.5f; } }

        public override string DisplayName { get { return "Bread"; } }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { new ItemStack(WheatItem.ItemID), new ItemStack(WheatItem.ItemID), new ItemStack(WheatItem.ItemID) },
                };
            }
        }

        public ItemStack Output
        {
            get { return new ItemStack(ItemID); }
        }

        public bool SignificantMetadata
        {
            get { return false; }
        }
    }
}