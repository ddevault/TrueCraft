using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class BowItem : ItemProvider, ICraftingRecipe
    {
        public static readonly short ItemID = 0x105;

        public override short ID { get { return 0x105; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(5, 1);
        }

        public override sbyte MaximumStack { get { return 1; } }

        public override string DisplayName { get { return "Bow"; } }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { ItemStack.EmptyStack, new ItemStack(StickItem.ItemID), new ItemStack(StringItem.ItemID) },
                    { new ItemStack(StickItem.ItemID), ItemStack.EmptyStack, new ItemStack(StringItem.ItemID) },
                    { ItemStack.EmptyStack, new ItemStack(StickItem.ItemID), new ItemStack(StringItem.ItemID) },
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