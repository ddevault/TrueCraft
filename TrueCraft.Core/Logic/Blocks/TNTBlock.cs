using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks
{
    public class TNTBlock : BlockProvider, ICraftingRecipe
    {
        public static readonly byte BlockID = 0x2E;
        
        public override byte ID { get { return 0x2E; } }
        
        public override double BlastResistance { get { return 0; } }

        public override double Hardness { get { return 0; } }

        public override byte Luminance { get { return 0; } }
        
        public override string DisplayName { get { return "TNT"; } }

        public override SoundEffectClass SoundEffect
        {
            get
            {
                return SoundEffectClass.Grass;
            }
        }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(8, 0);
        }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    {
                        new ItemStack(GunpowderItem.ItemID),
                        new ItemStack(SandBlock.BlockID),
                        new ItemStack(GunpowderItem.ItemID)
                    },
                    {
                        new ItemStack(SandBlock.BlockID),
                        new ItemStack(GunpowderItem.ItemID),
                        new ItemStack(SandBlock.BlockID)
                    },
                    {
                        new ItemStack(GunpowderItem.ItemID),
                        new ItemStack(SandBlock.BlockID),
                        new ItemStack(GunpowderItem.ItemID)
                    }
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
}