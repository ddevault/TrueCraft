using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class CompassItem : ToolItem, ICraftingRecipe
    {
        public static readonly short ItemID = 0x159;

        public override short ID { get { return 0x159; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(6, 3);
        }

        public override string DisplayName { get { return "Compass"; } }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { ItemStack.EmptyStack, new ItemStack(IronIngotItem.ItemID), ItemStack.EmptyStack },
                    { new ItemStack(IronIngotItem.ItemID), new ItemStack(RedstoneItem.ItemID), new ItemStack(IronIngotItem.ItemID) },
                    { ItemStack.EmptyStack, new ItemStack(IronIngotItem.ItemID), ItemStack.EmptyStack },
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