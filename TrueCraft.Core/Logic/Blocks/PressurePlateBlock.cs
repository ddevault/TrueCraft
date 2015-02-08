using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Blocks
{
    public abstract class PressurePlateBlock : BlockProvider
    {
        public override double BlastResistance { get { return 2.5; } }

        public override double Hardness { get { return 0.5; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }
    }

    public class WoodenPressurePlateBlock : PressurePlateBlock, ICraftingRecipe
    {
        public static readonly byte BlockID = 0x48;
        
        public override byte ID { get { return 0x48; } }
        
        public override string DisplayName { get { return "Wooden Pressure Plate"; } }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    {new ItemStack(WoodenPlanksBlock.BlockID), new ItemStack(WoodenPlanksBlock.BlockID)}
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

    public class StonePressurePlateBlock : PressurePlateBlock, ICraftingRecipe
    {
        public static readonly byte BlockID = 0x46;

        public override byte ID { get { return 0x46; } }

        public override string DisplayName { get { return "Stone Pressure Plate"; } }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    {new ItemStack(StoneBlock.BlockID), new ItemStack(StoneBlock.BlockID)}
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