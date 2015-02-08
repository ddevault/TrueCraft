using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.Logic.Items
{
    public class MushroomStewItem : FoodItem, ICraftingRecipe
    {
        public static readonly short ItemID = 0x11A;

        public override short ID { get { return 0x11A; } }

        public override sbyte MaximumStack { get { return 1; } }

        public override float Restores { get { return 5; } }

        public override string DisplayName { get { return "Mushroom Stew"; } }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { new ItemStack(RedMushroomBlock.BlockID) },
                    { new ItemStack(BrownMushroomBlock.BlockID) },
                    { new ItemStack(BowlItem.ItemID) }
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