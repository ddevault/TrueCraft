using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Blocks
{
    public class StoneStairsBlock : WoodenStairsBlock, ICraftingRecipe
    {
        public static new readonly byte BlockID = 0x43;
        
        public override byte ID { get { return 0x43; } }
        
        public override double BlastResistance { get { return 30; } }

        public override double Hardness { get { return 0; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }

        public override byte LightModifier { get { return 255; } }
        
        public override string DisplayName { get { return "Stone Stairs"; } }

        public override ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { new ItemStack(StoneBlock.BlockID), ItemStack.EmptyStack, ItemStack.EmptyStack },
                    { new ItemStack(StoneBlock.BlockID), new ItemStack(StoneBlock.BlockID), ItemStack.EmptyStack },
                    { new ItemStack(StoneBlock.BlockID), new ItemStack(StoneBlock.BlockID), new ItemStack(StoneBlock.BlockID) }
                };
            }
        }

        public override ItemStack Output
        {
            get
            {
                return new ItemStack(BlockID);
            }
        }

        public override bool SignificantMetadata
        {
            get
            {
                return false;
            }
        }
    }
}