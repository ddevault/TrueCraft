using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks
{
    public class IronBlock : BlockProvider, ICraftingRecipe
    {
        public static readonly byte BlockID = 0x2A;
        
        public override byte ID { get { return 0x2A; } }
        
        public override double BlastResistance { get { return 30; } }

        public override double Hardness { get { return 5; } }

        public override byte Luminance { get { return 0; } }
        
        public override string DisplayName { get { return "Block of Iron"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(6, 1);
        }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    {new ItemStack(IronIngotItem.ItemID), new ItemStack(IronIngotItem.ItemID), new ItemStack(IronIngotItem.ItemID)},
                    {new ItemStack(IronIngotItem.ItemID), new ItemStack(IronIngotItem.ItemID), new ItemStack(IronIngotItem.ItemID)},
                    {new ItemStack(IronIngotItem.ItemID), new ItemStack(IronIngotItem.ItemID), new ItemStack(IronIngotItem.ItemID)}
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