using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.Logic.Items
{
    public class BoatItem : ItemProvider, ICraftingRecipe
    {
        public static readonly short ItemID = 0x14D;

        public override short ID { get { return 0x14D; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(8, 8);
        }

        public override sbyte MaximumStack { get { return 1; } }

        public override string DisplayName { get { return "Boat"; } }

        public virtual ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    {
                        new ItemStack(WoodenPlanksBlock.BlockID),
                        ItemStack.EmptyStack,
                        new ItemStack(WoodenPlanksBlock.BlockID),
                    },
                    {
                        new ItemStack(WoodenPlanksBlock.BlockID),
                        new ItemStack(WoodenPlanksBlock.BlockID),
                        new ItemStack(WoodenPlanksBlock.BlockID)
                    }
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
            get { return false; }
        }
    }
}