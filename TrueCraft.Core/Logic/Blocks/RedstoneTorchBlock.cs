using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks
{
    public class RedstoneTorchBlock : TorchBlock, ICraftingRecipe
    {
        public static readonly new byte BlockID = 0x4C;
        
        public override byte ID { get { return 0x4C; } }
        
        public override double BlastResistance { get { return 0; } }

        public override double Hardness { get { return 0; } }

        public override byte Luminance { get { return 7; } }

        public override bool Opaque { get { return false; } }
        
        public override string DisplayName { get { return "Redstone Torch"; } }

        public override SoundEffectClass SoundEffect
        {
            get
            {
                return SoundEffectClass.Wood;
            }
        }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(3, 6);
        }

        public override ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { new ItemStack(RedstoneDustBlock.BlockID) },
                    { new ItemStack(StickItem.ItemID) }
                };
            }
        }

        public override ItemStack Output
        {
            get
            {
                return new ItemStack(BlockID);
            }
        }

        public override bool SignificantMetadata
        {
            get
            {
                return false;
            }
        }
    }

    public class InactiveRedstoneTorchBlock : RedstoneTorchBlock
    {
        public static readonly new byte BlockID = 0x4B;

        public override byte ID { get { return 0x4B; } }

        public override byte Luminance { get { return 0; } }

        public override string DisplayName { get { return "Redstone Torch (inactive)"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(3, 7);
        }
    }
}