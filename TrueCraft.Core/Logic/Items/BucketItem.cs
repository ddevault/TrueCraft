using System;
using TrueCraft.API.Logic;
using TrueCraft.API.Networking;
using TrueCraft.API.World;
using TrueCraft.API;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.Logic.Items
{
    public class BucketItem : ToolItem
    {
        public static readonly short ItemID = 0x145;

        public override short ID { get { return 0x145; } }

        public override string DisplayName { get { return "Bucket"; } }

        protected virtual byte? RelevantBlockType { get { return null; } }

        public override void ItemUsedOnBlock(Coordinates3D coordinates, ItemStack item, BlockFace face, IWorld world, IRemoteClient user)
        {
            coordinates += MathHelper.BlockFaceToCoordinates(face);
            if (item.ID == ItemID) // Empty bucket
            {
                var block = world.GetBlockID(coordinates);
                if (block == WaterBlock.BlockID || block == StationaryWaterBlock.BlockID)
                {
                    var meta = world.GetMetadata(coordinates);
                    if (meta == 0) // Is source block?
                    {
                        user.Inventory[user.SelectedSlot] = new ItemStack(WaterBucketItem.ItemID);
                        world.SetBlockID(coordinates, 0);
                    }
                }
                else if (block == LavaBlock.BlockID || block == StationaryLavaBlock.BlockID)
                {
                    var meta = world.GetMetadata(coordinates);
                    if (meta == 0) // Is source block?
                    {
                        user.Inventory[user.SelectedSlot] = new ItemStack(LavaBucketItem.ItemID);
                        world.SetBlockID(coordinates, 0);
                    }
                }
            }
            else
            {
                var provider = user.Server.BlockRepository.GetBlockProvider(world.GetBlockID(coordinates));
                if (!provider.Opaque)
                {
                    if (RelevantBlockType != null)
                    {
                        var blockType = RelevantBlockType.Value;
                        user.Server.BlockUpdatesEnabled = false;
                        world.SetBlockID(coordinates, blockType);
                        world.SetMetadata(coordinates, 0); // Source block
                        user.Server.BlockUpdatesEnabled = true;
                        var liquidProvider = world.BlockRepository.GetBlockProvider(blockType);
                        liquidProvider.BlockPlaced(new BlockDescriptor { Coordinates = coordinates }, face, world, user);
                    }
                    user.Inventory[user.SelectedSlot] = new ItemStack(BucketItem.ItemID);
                }
            }
        }
    }

    public class LavaBucketItem : BucketItem
    {
        public static readonly new short ItemID = 0x147;

        public override short ID { get { return 0x147; } }

        public override string DisplayName { get { return "Lava Bucket"; } }

        protected override byte? RelevantBlockType
        {
            get
            {
                return LavaBlock.BlockID;
            }
        }
    }

    public class MilkItem : BucketItem
    {
        public static readonly new short ItemID = 0x14F;

        public override short ID { get { return 0x14F; } }

        public override string DisplayName { get { return "Milk"; } }

        protected override byte? RelevantBlockType
        {
            get
            {
                return null;
            }
        }
    }

    public class WaterBucketItem : BucketItem
    {
        public static readonly new short ItemID = 0x146;

        public override short ID { get { return 0x146; } }

        public override string DisplayName { get { return "Water Bucket"; } }

        protected override byte? RelevantBlockType
        {
            get
            {
                return WaterBlock.BlockID;
            }
        }
    }
}