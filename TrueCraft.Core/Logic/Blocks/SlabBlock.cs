using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Blocks
{
    public class SlabBlock : BlockProvider
    {
        public enum SlabMaterial
        {
            Stone       = 0x0,
            Standstone  = 0x1,
            Wooden      = 0x2,
            Cobblestone = 0x3
        }

        public static readonly byte BlockID = 0x2C;
        
        public override byte ID { get { return 0x2C; } }
        
        public override double BlastResistance { get { return 30; } }

        public override double Hardness { get { return 2; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }

        public override byte LightOpacity { get { return 255; } }

        public override string DisplayName { get { return "Stone Slab"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(6, 0);
        }

        public class StoneSlabRecipe : ICraftingRecipe
        {
            public ItemStack[,] Pattern
            {
                get
                {
                    return new[,] { { new ItemStack(StoneBlock.BlockID), new ItemStack(StoneBlock.BlockID), new ItemStack(StoneBlock.BlockID) } };
                }
            }

            public ItemStack Output
            {
                get
                {
                    return new ItemStack(BlockID, 3, (short)SlabMaterial.Stone);
                }
            }

            public bool SignificantMetadata { get { return true; } }
        }

        public class StandstoneSlabRecipe : ICraftingRecipe
        {
            public ItemStack[,] Pattern
            {
                get
                {
                    return new[,] { { new ItemStack(SandstoneBlock.BlockID), new ItemStack(SandstoneBlock.BlockID), new ItemStack(SandstoneBlock.BlockID) } };
                }
            }

            public ItemStack Output
            {
                get
                {
                    return new ItemStack(BlockID, 3, (short)SlabMaterial.Standstone);
                }
            }

            public bool SignificantMetadata { get { return true; } }
        }

        public class WoodenSlabRecipe : ICraftingRecipe
        {
            public ItemStack[,] Pattern
            {
                get
                {
                    return new[,] { { new ItemStack(WoodenPlanksBlock.BlockID), new ItemStack(WoodenPlanksBlock.BlockID), new ItemStack(WoodenPlanksBlock.BlockID) } };
                }
            }

            public ItemStack Output
            {
                get
                {
                    return new ItemStack(BlockID, 3, (short)SlabMaterial.Wooden);
                }
            }

            public bool SignificantMetadata { get { return true; } }
        }

        public class CobblestoneSlabRecipe : ICraftingRecipe
        {
            public ItemStack[,] Pattern
            {
                get
                {
                    return new[,] { { new ItemStack(CobblestoneBlock.BlockID), new ItemStack(CobblestoneBlock.BlockID), new ItemStack(CobblestoneBlock.BlockID) } };
                }
            }

            public ItemStack Output
            {
                get
                {
                    return new ItemStack(BlockID, 3, (short)SlabMaterial.Cobblestone);
                }
            }

            public bool SignificantMetadata { get { return true; } }
        }
    }

    public class DoubleSlabBlock : SlabBlock
    {
        public static readonly new byte BlockID = 0x2B;

        public override byte ID { get { return 0x2B; } }

        public override double BlastResistance { get { return 30; } }

        public override double Hardness { get { return 2; } }

        public override byte Luminance { get { return 0; } }

        public override string DisplayName { get { return "Double Stone Slab"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(6, 0);
        }
    }
}