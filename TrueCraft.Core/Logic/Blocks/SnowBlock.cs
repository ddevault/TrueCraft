using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks
{
    public class SnowBlock : BlockProvider, ICraftingRecipe
    {
        public static readonly byte BlockID = 0x50;
        
        public override byte ID { get { return 0x50; } }
        
        public override double BlastResistance { get { return 1; } }

        public override double Hardness { get { return 0.2; } }

        public override byte Luminance { get { return 0; } }
        
        public override string DisplayName { get { return "Snow Block"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(2, 4);
        }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { new ItemStack(SnowballItem.ItemID), new ItemStack(SnowballItem.ItemID) },
                    { new ItemStack(SnowballItem.ItemID), new ItemStack(SnowballItem.ItemID) }
                };
            }
        }

        public ItemStack Output
        {
            get
            {
                return new ItemStack(BlockID);
            }
        }

        public bool SignificantMetadata
        {
            get
            {
                return false;
            }
        }
    }

    public class SnowfallBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x4E;

        public override byte ID { get { return 0x4E; } }

        public override double BlastResistance { get { return 0.5; } }

        public override double Hardness { get { return 0; } }

        public override byte Luminance { get { return 0; } }

        public override bool RenderOpaque { get { return true; } }

        public override bool Opaque { get { return false; } }

        public override string DisplayName { get { return "Snow"; } }

        public override TrueCraft.API.BoundingBox? BoundingBox { get { return null; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(2, 4);
        }

        public override ToolType EffectiveTools
        {
            get
            {
                return ToolType.Shovel;
            }
        }

        protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
        {
            return new[] { new ItemStack(SnowballItem.ItemID) };
        }
    }
}
