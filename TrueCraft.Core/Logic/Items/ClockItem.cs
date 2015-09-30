using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class ClockItem : ToolItem, ICraftingRecipe
    {
        public static readonly short ItemID = 0x15B;

        public override short ID { get { return 0x15B; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(6, 4);
        }

        public override string DisplayName { get { return "Clock"; } }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { ItemStack.EmptyStack, new ItemStack(GoldIngotItem.ItemID), ItemStack.EmptyStack },
                    { new ItemStack(GoldIngotItem.ItemID), new ItemStack(RedstoneItem.ItemID), new ItemStack(GoldIngotItem.ItemID) },
                    { ItemStack.EmptyStack, new ItemStack(GoldIngotItem.ItemID), ItemStack.EmptyStack },
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