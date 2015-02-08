using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks
{
    public class RailBlock : BlockProvider, ICraftingRecipe
    {
        public static readonly byte BlockID = 0x42;
        
        public override byte ID { get { return 0x42; } }
        
        public override double BlastResistance { get { return 3.5; } }

        public override double Hardness { get { return 0.7; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }
        
        public override string DisplayName { get { return "Rail"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(0, 8);
        }

        public virtual ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    {
                        new ItemStack(IronIngotItem.ItemID),
                        ItemStack.EmptyStack,
                        new ItemStack(IronIngotItem.ItemID),
                    },
                    {
                        new ItemStack(IronIngotItem.ItemID),
                        new ItemStack(StickItem.ItemID),
                        new ItemStack(IronIngotItem.ItemID),
                    },
                    {
                        new ItemStack(IronIngotItem.ItemID),
                        ItemStack.EmptyStack,
                        new ItemStack(IronIngotItem.ItemID),
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
            get { return false; }
        }
    }

    public class PoweredRailBlock : RailBlock
    {
        public static readonly new byte BlockID = 0x1B;

        public override byte ID { get { return 0x1B; } }

        public override string DisplayName { get { return "Powered Rail"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(3, 11);
        }

        public override ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    {
                        new ItemStack(IronIngotItem.ItemID),
                        ItemStack.EmptyStack,
                        new ItemStack(IronIngotItem.ItemID),
                    },
                    {
                        new ItemStack(IronIngotItem.ItemID),
                        new ItemStack(StickItem.ItemID),
                        new ItemStack(IronIngotItem.ItemID),
                    },
                    {
                        new ItemStack(IronIngotItem.ItemID),
                        new ItemStack(RedstoneDustBlock.BlockID),
                        new ItemStack(IronIngotItem.ItemID),
                    }
                };
            }
        }
    }

    public class DetectorRailBlock : RailBlock
    {
        public static readonly new byte BlockID = 0x1C;

        public override byte ID { get { return 0x1C; } }

        public override string DisplayName { get { return "Detector Rail"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(3, 12);
        }

        public override ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    {
                        new ItemStack(IronIngotItem.ItemID),
                        ItemStack.EmptyStack,
                        new ItemStack(IronIngotItem.ItemID),
                    },
                    {
                        new ItemStack(IronIngotItem.ItemID),
                        new ItemStack(StonePressurePlateBlock.BlockID),
                        new ItemStack(IronIngotItem.ItemID),
                    },
                    {
                        new ItemStack(IronIngotItem.ItemID),
                        new ItemStack(RedstoneDustBlock.BlockID),
                        new ItemStack(IronIngotItem.ItemID),
                    }
                };
            }
        }
    }
}