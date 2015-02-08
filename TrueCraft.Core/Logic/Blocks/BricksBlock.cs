using System;
using TrueCraft.API;
using TrueCraft.API.Logic;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks
{
    public class BricksBlock : BlockProvider, ICraftingRecipe
    {
        public static readonly byte BlockID = 0x2D;
        
        public override byte ID { get { return 0x2D; } }
        
        public override double BlastResistance { get { return 30; } }

        public override double Hardness { get { return 2; } }

        public override byte Luminance { get { return 0; } }
        
        public override string DisplayName { get { return "Bricks"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(7, 0);
        }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    {new ItemStack(BrickItem.ItemID), new ItemStack(BrickItem.ItemID)},
                    {new ItemStack(BrickItem.ItemID), new ItemStack(BrickItem.ItemID)}
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