using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.API.World;
using TrueCraft.API.Networking;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.Logic.Items
{
    public class SeedsItem : ItemProvider
    {
        public static readonly short ItemID = 0x127;

        public override short ID { get { return 0x127; } }

        public override string DisplayName { get { return "Seeds"; } }

        public override void ItemUsedOnBlock(Coordinates3D coordinates, ItemStack item, BlockFace face, IWorld world, IRemoteClient user)
        {
            if (world.GetBlockID(coordinates) == FarmlandBlock.BlockID)
            {
                world.SetBlockID(coordinates + MathHelper.BlockFaceToCoordinates(face), CropsBlock.BlockID);
                world.BlockRepository.GetBlockProvider(CropsBlock.BlockID).BlockPlaced(
                    new BlockDescriptor { Coordinates = coordinates }, face, world, user);
            }
        }
    }
}