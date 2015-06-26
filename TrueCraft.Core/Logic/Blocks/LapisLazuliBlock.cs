using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks
{
    public class LapisLazuliBlock : BlockProvider, ICraftingRecipe
    {
        public static readonly byte BlockID = 0x16;
        
        public override byte ID { get { return 0x16; } }
        
        public override double BlastResistance { get { return 15; } }

        public override double Hardness { get { return 3; } }

        public override byte Luminance { get { return 0; } }
        
        public override string DisplayName { get { return "Lapis Lazuli Block"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(0, 9);
        }

        public ItemStack[,] Pattern
        {
            get
            {
                return new [,]
                {
                    {
                        new ItemStack(DyeItem.ItemID, 1, (short)DyeItem.DyeType.LapisLazuli),
                        new ItemStack(DyeItem.ItemID, 1, (short)DyeItem.DyeType.LapisLazuli),
                        new ItemStack(DyeItem.ItemID, 1, (short)DyeItem.DyeType.LapisLazuli)
                    },
                    {
                        new ItemStack(DyeItem.ItemID, 1, (short)DyeItem.DyeType.LapisLazuli),
                        new ItemStack(DyeItem.ItemID, 1, (short)DyeItem.DyeType.LapisLazuli),
                        new ItemStack(DyeItem.ItemID, 1, (short)DyeItem.DyeType.LapisLazuli)
                    },
                    {
                        new ItemStack(DyeItem.ItemID, 1, (short)DyeItem.DyeType.LapisLazuli),
                        new ItemStack(DyeItem.ItemID, 1, (short)DyeItem.DyeType.LapisLazuli),
                        new ItemStack(DyeItem.ItemID, 1, (short)DyeItem.DyeType.LapisLazuli)
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
            get { return true; }
        }

        public override ToolMaterial EffectiveToolMaterials
        {
            get
            {
                return ToolMaterial.Stone | ToolMaterial.Iron | ToolMaterial.Diamond;
            }
        }

        public override ToolType EffectiveTools
        {
            get
            {
                return ToolType.Pickaxe;
            }
        }
    }
}