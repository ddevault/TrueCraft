using System;
using TrueCraft.API.Logic;
using TrueCraft.API;


namespace TrueCraft.Core.Logic.Items
{
    public class FlintAndSteelItem : ToolItem, ICraftingRecipe
    {
        public static readonly short ItemID = 0x103;
        public override short ID { get { return 0x103; } }
        public override sbyte MaximumStack { get { return 1; } }
        public override short BaseDurability { get { return 65; } }
        public override string DisplayName { get { return "Flint and Steel"; } }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { new ItemStack(IronIngotItem.ItemID), new ItemStack(FlintItem.ItemID) }
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
