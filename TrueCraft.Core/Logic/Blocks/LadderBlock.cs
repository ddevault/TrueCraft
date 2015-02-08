using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks
{
    public class LadderBlock : BlockProvider, ICraftingRecipe
    {
        public static readonly byte BlockID = 0x41;
        
        public override byte ID { get { return 0x41; } }
        
        public override double BlastResistance { get { return 2; } }

        public override double Hardness { get { return 0.4; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }
        
        public override string DisplayName { get { return "Ladder"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(3, 5);
        }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    {
                        new ItemStack(StickItem.ItemID),
                        ItemStack.EmptyStack,
                        new ItemStack(StickItem.ItemID)
                    },
                    {
                        new ItemStack(StickItem.ItemID),
                        new ItemStack(StickItem.ItemID),
                        new ItemStack(StickItem.ItemID)
                    },
                    {
                        new ItemStack(StickItem.ItemID),
                        ItemStack.EmptyStack,
                        new ItemStack(StickItem.ItemID)
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