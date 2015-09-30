using System;
using System.Linq;
using TrueCraft.API;
using TrueCraft.API.Logic;
using TrueCraft.API.Networking;
using TrueCraft.API.World;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.Logic.Items
{
    public class RedstoneItem : ItemProvider
    {
        public static readonly short ItemID = 0x14B;

        public override short ID { get { return 0x14B; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(8, 3);
        }

        public override string DisplayName { get { return "Redstone"; } }

        public override void ItemUsedOnBlock(Coordinates3D coordinates, ItemStack item, BlockFace face, IWorld world, IRemoteClient user)
        {
            coordinates += MathHelper.BlockFaceToCoordinates(face);
            IBlockProvider supportingBlock = world.BlockRepository.GetBlockProvider(world.GetBlockID(coordinates + Coordinates3D.Down));

            if (supportingBlock.Opaque)
            {
                world.SetBlockID(coordinates, RedstoneDustBlock.BlockID);
                item.Count--;
                user.Inventory[user.SelectedSlot] = item;
            }
        }
    }
}