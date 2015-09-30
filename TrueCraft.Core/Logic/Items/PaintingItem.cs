using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.Logic.Items
{
    public class PaintingItem : ItemProvider, ICraftingRecipe
    {
        public static readonly short ItemID = 0x141;

        public override short ID { get { return 0x141; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(10, 1);
        }

        public override string DisplayName { get { return "Painting"; } }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { new ItemStack(StickItem.ItemID), new ItemStack(StickItem.ItemID), new ItemStack(StickItem.ItemID) },
                    { new ItemStack(StickItem.ItemID), new ItemStack(WoolBlock.BlockID), new ItemStack(StickItem.ItemID) },
                    { new ItemStack(StickItem.ItemID), new ItemStack(StickItem.ItemID), new ItemStack(StickItem.ItemID) }
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