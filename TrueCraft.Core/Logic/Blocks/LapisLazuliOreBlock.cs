using System;
using TrueCraft.API;
using TrueCraft.API.Logic;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks
{
    public class LapisLazuliOreBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x15;
        
        public override byte ID { get { return 0x15; } }
        
        public override double BlastResistance { get { return 15; } }

        public override double Hardness { get { return 3; } }

        public override byte Luminance { get { return 0; } }
        
        public override string DisplayName { get { return "Lapis Lazuli Ore"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(0, 10);
        }

        protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
        {
            return new[] { new ItemStack(DyeItem.ItemID, (sbyte)new Random().Next(4, 8), (short)DyeItem.DyeType.LapisLazuli) };
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