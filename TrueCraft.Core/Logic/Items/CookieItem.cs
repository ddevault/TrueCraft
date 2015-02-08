using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class CookieItem : FoodItem, ICraftingRecipe
    {
        public static readonly short ItemID = 0x165;

        public override short ID { get { return 0x165; } }

        public override sbyte MaximumStack { get { return 8; } }

        public override float Restores { get { return 0.5f; } }

        public override string DisplayName { get { return "Cookie"; } }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { 
                        new ItemStack(WheatItem.ItemID),
                        new ItemStack(DyeItem.ItemID, 1, (short)DyeItem.DyeType.CocoaBeans), 
                        new ItemStack(WheatItem.ItemID) 
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