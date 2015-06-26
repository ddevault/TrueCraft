using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Blocks
{
    public class ObsidianBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x31;
        
        public override byte ID { get { return 0x31; } }
        
        public override double BlastResistance { get { return 6000; } }

        public override double Hardness { get { return 10; } }

        public override byte Luminance { get { return 0; } }
        
        public override string DisplayName { get { return "Obsidian"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(5, 2);
        }

        public override ToolMaterial EffectiveToolMaterials
        {
            get
            {
                return ToolMaterial.Diamond;
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