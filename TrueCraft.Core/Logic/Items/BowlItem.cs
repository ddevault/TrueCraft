using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.Logic.Items
{
    public class BowlItem : ItemProvider, ICraftingRecipe
    {
        public static readonly short ItemID = 0x119;

        public override short ID { get { return 0x119; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(7, 4);
        }

        public override string DisplayName { get { return "Bowl"; } }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { new ItemStack(WoodenPlanksBlock.BlockID), ItemStack.EmptyStack, new ItemStack(WoodenPlanksBlock.BlockID) },
                    { ItemStack.EmptyStack, new ItemStack(WoodenPlanksBlock.BlockID), ItemStack.EmptyStack }
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