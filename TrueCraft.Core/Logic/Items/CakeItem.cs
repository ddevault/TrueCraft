using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.API.World;
using TrueCraft.API.Networking;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.Logic.Items
{
    public class CakeItem : FoodItem, ICraftingRecipe // TODO: This isn't really a FoodItem
    {
        public static readonly short ItemID = 0x162;

        public override short ID { get { return 0x162; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(13, 1);
        }

        //This is per "slice"
        public override float Restores { get { return 1.5f; } }

        public override string DisplayName { get { return "Cake"; } }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    { new ItemStack(MilkItem.ItemID), new ItemStack(MilkItem.ItemID), new ItemStack(MilkItem.ItemID) },
                    { new ItemStack(SugarItem.ItemID), new ItemStack(EggItem.ItemID), new ItemStack(SugarItem.ItemID) },
                    { new ItemStack(WheatItem.ItemID), new ItemStack(WheatItem.ItemID), new ItemStack(WheatItem.ItemID) }
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
                return false;
            }
        }

        public override void ItemUsedOnBlock(Coordinates3D coordinates, ItemStack item, BlockFace face, IWorld world, IRemoteClient user)
        {
            coordinates += MathHelper.BlockFaceToCoordinates(face);
            var old = world.BlockRepository.GetBlockProvider(world.GetBlockID(coordinates));
            if (old.Hardness == 0)
            {
                world.SetBlockID(coordinates, CakeBlock.BlockID);
                item.Count--;
                user.Inventory[user.SelectedSlot] = item;
            }
        }
    }
}