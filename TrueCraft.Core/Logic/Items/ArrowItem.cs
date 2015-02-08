using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class ArrowItem : ItemProvider, ICraftingRecipe
    {
        public static readonly short ItemID = 0x106;

        public override short ID { get { return 0x106; } }

        public override string DisplayName { get { return "Arrow"; } }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { new ItemStack(FlintItem.ItemID) },
                    { new ItemStack(StickItem.ItemID) },
                    { new ItemStack(FeatherItem.ItemID) }
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