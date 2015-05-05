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
        /// <summary>
        /// Redstone cannot be placed on any of the following BlockIDs
        /// </summary>
        protected static byte[] cannotPlace = { RedstoneDustBlock.BlockID, GlassBlock.BlockID, AirBlock.BlockID };

        public static readonly short ItemID = 0x14B;

        public override short ID { get { return 0x14B; } }

        public override string DisplayName { get { return "Redstone"; } }

        public override void ItemUsedOnBlock(Coordinates3D coordinates, ItemStack item, BlockFace face, IWorld world, IRemoteClient user)
        {
            // TODO: Running backwards and spamming redstone dust sometimes causes redstone to be placed on redstone

            if (face != BlockFace.PositiveY)
            {
                // Redstone dust cannot be placed anywhere but the top of a block
                return;
            }

            IBlockProvider clickedBlock = world.BlockRepository.GetBlockProvider(world.GetBlockID(coordinates));
            if (null != clickedBlock)
            {
                if (cannotPlace.Any(b => b == clickedBlock.ID))
                {
                    return;
                }

                coordinates += MathHelper.BlockFaceToCoordinates(face);

                world.SetBlockID(coordinates, RedstoneDustBlock.BlockID);
                item.Count--;
                user.Inventory[user.SelectedSlot] = item;
            }
        }
    }
}