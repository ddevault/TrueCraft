using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.API.World;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Logic.Items
{
    public class SignItem : ItemProvider, ICraftingRecipe
    {
        public static readonly short ItemID = 0x143;

        public override short ID { get { return 0x143; } }

        public override sbyte MaximumStack { get { return 1; } }

        public override string DisplayName { get { return "Sign"; } }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { new ItemStack(WoodenPlanksBlock.BlockID), new ItemStack(WoodenPlanksBlock.BlockID), new ItemStack(WoodenPlanksBlock.BlockID) },
                    { new ItemStack(WoodenPlanksBlock.BlockID), new ItemStack(WoodenPlanksBlock.BlockID), new ItemStack(WoodenPlanksBlock.BlockID) },
                    { ItemStack.EmptyStack, new ItemStack(StickItem.ItemID), ItemStack.EmptyStack }
                };
            }
        }

        public ItemStack Output
        {
            get
            {
                return new ItemStack(ItemID);
            }
        }

        public bool SignificantMetadata
        {
            get
            {
                return true;
            }
        }

        public override void ItemUsedOnBlock(Coordinates3D coordinates, ItemStack item, BlockFace face, IWorld world, IRemoteClient user)
        {
            if (face == BlockFace.PositiveY)
            {
                var provider = user.Server.BlockRepository.GetBlockProvider(UprightSignBlock.BlockID);
                (provider as IItemProvider).ItemUsedOnBlock(coordinates, item, face, world, user);
            }
            else
            {
                var provider = user.Server.BlockRepository.GetBlockProvider(WallSignBlock.BlockID);
                (provider as IItemProvider).ItemUsedOnBlock(coordinates, item, face, world, user);
            }
        }
    }
}