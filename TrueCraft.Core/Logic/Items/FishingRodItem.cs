using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class FishingRodItem : ToolItem, ICraftingRecipe
    {
        public static readonly short ItemID = 0x15A;

        public override short ID { get { return 0x15A; } }

        public override sbyte MaximumStack { get { return 1; } }

        public override short BaseDurability { get { return 65; } }

        public override string DisplayName { get { return "Fishing Rod"; } }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { ItemStack.EmptyStack, ItemStack.EmptyStack, new ItemStack(StickItem.ItemID) },
                    { ItemStack.EmptyStack, new ItemStack(StickItem.ItemID), new ItemStack(StringItem.ItemID) },
                    { new ItemStack(StickItem.ItemID), ItemStack.EmptyStack, new ItemStack(StringItem.ItemID) },
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