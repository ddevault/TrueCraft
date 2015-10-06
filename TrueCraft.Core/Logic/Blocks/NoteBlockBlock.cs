using System;
using TrueCraft.API;
using TrueCraft.API.Logic;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks
{
    public class NoteBlockBlock : BlockProvider, ICraftingRecipe, IBurnableItem
    {
        public static readonly byte BlockID = 0x19;
        
        public override byte ID { get { return 0x19; } }
        
        public override double BlastResistance { get { return 4; } }

        public override double Hardness { get { return 0.8; } }

        public override byte Luminance { get { return 0; } }
        
        public override string DisplayName { get { return "Note Block"; } }

        public TimeSpan BurnTime { get { return TimeSpan.FromSeconds(15); } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(10, 4);
        }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    {
                        new ItemStack(WoodenPlanksBlock.BlockID),
                        new ItemStack(WoodenPlanksBlock.BlockID),
                        new ItemStack(WoodenPlanksBlock.BlockID)
                    },
                    {
                        new ItemStack(WoodenPlanksBlock.BlockID),
                        new ItemStack(RedstoneItem.ItemID),
                        new ItemStack(WoodenPlanksBlock.BlockID)
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
            get { return new ItemStack(BlockID); }
        }

        public bool SignificantMetadata
        {
            get { return false; }
        }
    }
}