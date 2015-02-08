using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Blocks
{
    public class JackoLanternBlock : BlockProvider, ICraftingRecipe
    {
        public static readonly byte BlockID = 0x5B;
        
        public override byte ID { get { return 0x5B; } }
        
        public override double BlastResistance { get { return 5; } }

        public override double Hardness { get { return 1; } }

        public override byte Luminance { get { return 15; } }

        public override bool Opaque { get { return false; } }

        public override byte LightModifier { get { return 255; } }
        
        public override string DisplayName { get { return "Jack 'o' Lantern"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(6, 6);
        }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { new ItemStack(PumpkinBlock.BlockID) },
                    { new ItemStack(TorchBlock.BlockID) }
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