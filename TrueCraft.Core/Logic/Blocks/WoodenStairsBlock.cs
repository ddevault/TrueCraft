using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Blocks
{
    public class WoodenStairsBlock : BlockProvider, ICraftingRecipe
    {
        public static readonly byte BlockID = 0x35;
        
        public override byte ID { get { return 0x35; } }
        
        public override double BlastResistance { get { return 15; } }

        public override double Hardness { get { return 0; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }

        public override byte LightModifier { get { return 255; } }
        
        public override string DisplayName { get { return "Wooden Stairs"; } }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { new ItemStack(WoodenPlanksBlock.BlockID), ItemStack.EmptyStack, ItemStack.EmptyStack },
                    { new ItemStack(WoodenPlanksBlock.BlockID), new ItemStack(WoodenPlanksBlock.BlockID), ItemStack.EmptyStack },
                    { new ItemStack(WoodenPlanksBlock.BlockID), new ItemStack(WoodenPlanksBlock.BlockID), new ItemStack(WoodenPlanksBlock.BlockID) }
                };
            }
        }

        public ItemStack Output
        {
            get
            {
                return new ItemStack(BlockID);
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