using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class CakeItem : FoodItem, ICraftingRecipe // TODO: This isn't really a FoodItem
    {
        public static readonly short ItemID = 0x162;

        public override short ID { get { return 0x162; } }

        //This is per "slice"
        public override float Restores { get { return 1.5f; } }

        public override string DisplayName { get { return "Cake"; } }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { new ItemStack(MilkItem.ItemID), new ItemStack(MilkItem.ItemID), new ItemStack(MilkItem.ItemID) },
                    { new ItemStack(SugarItem.ItemID), new ItemStack(EggItem.ItemID), new ItemStack(SugarItem.ItemID) },
                    { new ItemStack(WheatItem.ItemID), new ItemStack(WheatItem.ItemID), new ItemStack(WheatItem.ItemID) }
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
                return false;
            }
        }
    }
}