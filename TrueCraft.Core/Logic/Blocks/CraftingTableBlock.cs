using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.API.World;
using TrueCraft.API.Networking;
using TrueCraft.Core.Windows;

namespace TrueCraft.Core.Logic.Blocks
{
    public class CraftingTableBlock : BlockProvider, ICraftingRecipe, IBurnableItem
    {
        public static readonly byte BlockID = 0x3A;
        
        public override byte ID { get { return 0x3A; } }
        
        public override double BlastResistance { get { return 12.5; } }

        public override double Hardness { get { return 2.5; } }

        public override byte Luminance { get { return 0; } }
        
        public override string DisplayName { get { return "Crafting Table"; } }

        public TimeSpan BurnTime { get { return TimeSpan.FromSeconds(15); } }

        public override SoundEffectClass SoundEffect
        {
            get
            {
                return SoundEffectClass.Wood;
            }
        }

        public override bool BlockRightClicked(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            var window = new CraftingBenchWindow(user.Server.CraftingRepository, (InventoryWindow)user.Inventory);
            user.OpenWindow(window);
            return false;
        }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(11, 3);
        }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { new ItemStack(WoodenPlanksBlock.BlockID), new ItemStack(WoodenPlanksBlock.BlockID) },
                    { new ItemStack(WoodenPlanksBlock.BlockID), new ItemStack(WoodenPlanksBlock.BlockID) },
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
}