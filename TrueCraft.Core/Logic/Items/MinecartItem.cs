using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.Logic.Items
{
    public class MinecartItem : ItemProvider, ICraftingRecipe
    {
        public static readonly short ItemID = 0x148;

        public override short ID { get { return 0x148; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(7, 8);
        }

        public override sbyte MaximumStack { get { return 1; } }

        public override string DisplayName { get { return "Minecart"; } }

        public virtual ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { new ItemStack(IronIngotItem.ItemID), ItemStack.EmptyStack, new ItemStack(IronIngotItem.ItemID) },
                    { new ItemStack(IronIngotItem.ItemID), new ItemStack(IronIngotItem.ItemID), new ItemStack(IronIngotItem.ItemID) },
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

    public class MinecartWithChestItem : MinecartItem, ICraftingRecipe
    {
        public static readonly new short ItemID = 0x156;

        public override short ID { get { return 0x156; } }

        public override string DisplayName { get { return "Minecart with Chest"; } }

        public override ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { new ItemStack(ChestBlock.BlockID)  },
                    { new ItemStack(MinecartItem.ItemID) },
                };
            }
        }
    }

    public class MinecartWithFurnaceItem : MinecartItem, ICraftingRecipe
    {
        public static readonly new short ItemID = 0x157;

        public override short ID { get { return 0x157; } }

        public override string DisplayName { get { return "Minecart with Furnace"; } }

        public override ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { new ItemStack(FurnaceBlock.BlockID)  },
                    { new ItemStack(MinecartItem.ItemID) },
                };
            }
        }
    }
}