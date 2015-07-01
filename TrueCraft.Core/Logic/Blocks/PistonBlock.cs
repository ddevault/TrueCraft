using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.Core.Logic.Items;
using TrueCraft.API.World;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Logic.Blocks
{
    public class PistonBlock : BlockProvider, ICraftingRecipe
    {
        public static readonly byte BlockID = 0x21;
        
        public override byte ID { get { return 0x21; } }
        
        public override double BlastResistance { get { return 2.5; } }

        public override double Hardness { get { return 0.5; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }
        
        public override string DisplayName { get { return "Piston"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(11, 6);
        }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    {
                        new ItemStack(WoodenPlanksBlock.BlockID),
                        new ItemStack(WoodenPlanksBlock.BlockID),
                        new ItemStack(WoodenPlanksBlock.BlockID)
                    },
                    {
                        new ItemStack(CobblestoneBlock.BlockID),
                        new ItemStack(IronIngotItem.ItemID),
                        new ItemStack(CobblestoneBlock.BlockID)
                    },
                    {
                        new ItemStack(CobblestoneBlock.BlockID),
                        new ItemStack(RedstoneItem.ItemID),
                        new ItemStack(CobblestoneBlock.BlockID)
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

        public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            world.SetMetadata(descriptor.Coordinates,
                (byte)MathHelper.DirectionByRotation(user.Entity.Position, user.Entity.Yaw,
                descriptor.Coordinates, true));
        }
    }

    public class StickyPistonBlock : BlockProvider, ICraftingRecipe
    {
        public static readonly byte BlockID = 0x1D;

        public override byte ID { get { return 0x1D; } }

        public override double BlastResistance { get { return 2.5; } }

        public override double Hardness { get { return 0.5; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }

        public override string DisplayName { get { return "Sticky Piston"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(10, 6);
        }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    {new ItemStack(SlimeballItem.ItemID)},
                    {new ItemStack(PistonBlock.BlockID)}
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

        public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            world.SetMetadata(descriptor.Coordinates,
                (byte)MathHelper.DirectionByRotation(user.Entity.Position, user.Entity.Yaw,
                descriptor.Coordinates, true));
        }
    }

    public class PistonPlungerBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x22;

        public override byte ID { get { return 0x22; } }

        public override double BlastResistance { get { return 2.5; } }

        public override double Hardness { get { return 0.5; } }

        public override byte Luminance { get { return 0; } }

        public override string DisplayName { get { return "Piston Plunger"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(11, 6);
        }
    }

    public class PistonPlaceholderBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x24;

        public override byte ID { get { return 0x24; } }

        public override double BlastResistance { get { return 0; } }

        public override double Hardness { get { return 0; } }

        public override byte Luminance { get { return 0; } }

        public override string DisplayName { get { return "Piston Placeholder"; } }
    }
}