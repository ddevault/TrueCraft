using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks
{
    public class GoldBlock : BlockProvider, ICraftingRecipe
    {
        public static readonly byte BlockID = 0x29;
        
        public override byte ID { get { return 0x29; } }
        
        public override double BlastResistance { get { return 30; } }

        public override double Hardness { get { return 3; } }

        public override byte Luminance { get { return 0; } }
        
        public override string DisplayName { get { return "Block of Gold"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(7, 1);
        }

        public ItemStack[,] Pattern
        {
            get
            {
                return new [,]
                {
                    {new ItemStack(GoldIngotItem.ItemID), new ItemStack(GoldIngotItem.ItemID), new ItemStack(GoldIngotItem.ItemID)},
                    {new ItemStack(GoldIngotItem.ItemID), new ItemStack(GoldIngotItem.ItemID), new ItemStack(GoldIngotItem.ItemID)},
                    {new ItemStack(GoldIngotItem.ItemID), new ItemStack(GoldIngotItem.ItemID), new ItemStack(GoldIngotItem.ItemID)}
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

        public override ToolMaterial EffectiveToolMaterials
        {
            get
            {
                return ToolMaterial.Iron | ToolMaterial.Diamond;
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