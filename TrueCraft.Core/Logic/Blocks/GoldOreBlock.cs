using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Blocks
{
    public class GoldOreBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x0E;
        
        public override byte ID { get { return 0x0E; } }
        
        public override double BlastResistance { get { return 15; } }

        public override double Hardness { get { return 3; } }

        public override byte Luminance { get { return 0; } }
        
        public override string DisplayName { get { return "Gold Ore"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(0, 2);
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