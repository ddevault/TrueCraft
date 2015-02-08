using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.Logic.Items
{
    public class RedstoneRepeaterItem : ItemProvider, ICraftingRecipe
    {
        public static readonly short ItemID = 0x164;

        public override short ID { get { return 0x164; } }

        public override string DisplayName { get { return "Redstone Repeater"; } }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { new ItemStack(RedstoneTorchBlock.BlockID), new ItemStack(RedstoneDustBlock.BlockID), new ItemStack(RedstoneTorchBlock.BlockID) },
                    { new ItemStack(StoneBlock.BlockID), new ItemStack(StoneBlock.BlockID), new ItemStack(StoneBlock.BlockID) }
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