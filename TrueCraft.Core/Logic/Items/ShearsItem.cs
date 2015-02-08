using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class ShearsItem : ToolItem, ICraftingRecipe
    {
        public static readonly short ItemID = 0x167;

        public override short ID { get { return 0x167; } }

        public override sbyte MaximumStack { get { return 1; } }

        public override short BaseDurability { get { return 239; } }

        public override string DisplayName { get { return "Shears"; } }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { ItemStack.EmptyStack, new ItemStack(IronIngotItem.ItemID) },
                    { new ItemStack(IronIngotItem.ItemID), ItemStack.EmptyStack }
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