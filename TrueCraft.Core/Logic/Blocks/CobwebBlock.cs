using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks
{
    public class CobwebBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x1E;
        
        public override byte ID { get { return 0x1E; } }
        
        public override double BlastResistance { get { return 20; } }

        public override double Hardness { get { return 4; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }

        public override bool DiffuseSkyLight
        {
            get
            {
                return true;
            }
        }

        public override ToolType EffectiveTools
        {
            get
            {
                return ToolType.Sword;
            }
        }
        
        public override string DisplayName { get { return "Cobweb"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(11, 0);
        }

        protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
        {
            return new[] { new ItemStack(StringItem.ItemID, 1, descriptor.Metadata) };
        }
    }
}