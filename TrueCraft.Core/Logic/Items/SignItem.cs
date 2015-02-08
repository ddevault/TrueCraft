using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.Logic.Items
{
    public class SignItem : ItemProvider, ICraftingRecipe
    {
        public static readonly short ItemID = 0x143;

        public override short ID { get { return 0x143; } }

        public override sbyte MaximumStack { get { return 1; } }

        public override string DisplayName { get { return "Sign"; } }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { new ItemStack(WoodenPlanksBlock.BlockID), new ItemStack(WoodenPlanksBlock.BlockID), new ItemStack(WoodenPlanksBlock.BlockID) },
                    { new ItemStack(WoodenPlanksBlock.BlockID), new ItemStack(WoodenPlanksBlock.BlockID), new ItemStack(WoodenPlanksBlock.BlockID) },
                    { ItemStack.EmptyStack, new ItemStack(StickItem.ItemID), ItemStack.EmptyStack }
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