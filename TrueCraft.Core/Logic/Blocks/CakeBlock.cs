using System;
using TrueCraft.API;
using TrueCraft.API.Logic;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks
{
    public class CakeBlock : BlockProvider, ICraftingRecipe
    {
        public static readonly byte BlockID = 0x5C;
        
        public override byte ID { get { return 0x5C; } }
        
        public override double BlastResistance { get { return 2.5; } }

        public override double Hardness { get { return 0.5; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }
        
        public override string DisplayName { get { return "Cake"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(9, 7);
        }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    {
                        new ItemStack(MilkItem.ItemID),
                        new ItemStack(MilkItem.ItemID),
                        new ItemStack(MilkItem.ItemID)
                    },
                    {
                        new ItemStack(SugarItem.ItemID),
                        new ItemStack(EggItem.ItemID),
                        new ItemStack(SugarItem.ItemID)
                    },
                    {
                        new ItemStack(WheatItem.ItemID),
                        new ItemStack(WheatItem.ItemID),
                        new ItemStack(WheatItem.ItemID)
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
            get { return false; }
        }
    }
}