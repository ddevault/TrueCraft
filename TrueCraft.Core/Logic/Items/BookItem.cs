using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class BookItem : ItemProvider, ICraftingRecipe
    {
        public static readonly short ItemID = 0x154;

        public override short ID { get { return 0x154; } }

        public override sbyte MaximumStack { get { return 64; } }

        public override string DisplayName { get { return "Book"; } }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { new ItemStack(PaperItem.ItemID) },
                    { new ItemStack(PaperItem.ItemID) },
                    { new ItemStack(PaperItem.ItemID) },
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