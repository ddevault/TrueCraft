using System;
using TrueCraft.API.Logic;
using TrueCraft.API.World;
using TrueCraft.API;
using TrueCraft.API.Networking;
using TrueCraft.Core.Entities;
using TrueCraft.API.Entities;
using TrueCraft.API.Server;
using TrueCraft.Core.Logic.Blocks;
using System.Linq;
using fNbt;

namespace TrueCraft.Core.Logic
{
    /// <summary>
    /// Provides common implementations of block logic.
    /// </summary>
    public abstract class BlockProvider : IItemProvider, IBlockProvider
    {
        public virtual void BlockLeftClicked(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            // This space intentionally left blank
        }

        public virtual bool BlockRightClicked(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            return true;
        }

        public virtual void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            // This space intentionally left blank
        }

        public virtual void BlockMined(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            GenerateDropEntity(descriptor, world, user.Server);
            world.SetBlockID(descriptor.Coordinates, 0);
        }

        public void GenerateDropEntity(BlockDescriptor descriptor, IWorld world, IMultiplayerServer server)
        {
            var entityManager = server.GetEntityManagerForWorld(world);
            var items = GetDrop(descriptor);
            foreach (var item in items)
            {
                if (item.Empty) continue;
                var entity = new ItemEntity(new Vector3(descriptor.Coordinates) + new Vector3(0.5), item);
                entityManager.SpawnEntity(entity);
            }
        }

        public virtual bool IsSupported(BlockDescriptor descriptor, IMultiplayerServer server, IWorld world)
        {
            var support = GetSupportDirection(descriptor);
            if (support != Coordinates3D.Zero)
            {
                var supportingBlock = server.BlockRepository.GetBlockProvider(world.GetBlockID(descriptor.Coordinates + support));
                if (!supportingBlock.Opaque)
                    return false;
            }
            return true;
        }

        public virtual void BlockUpdate(BlockDescriptor descriptor, BlockDescriptor source, IMultiplayerServer server, IWorld world)
        {
            if (!IsSupported(descriptor, server, world))
            {
                GenerateDropEntity(descriptor, world, server);
                world.SetBlockID(descriptor.Coordinates, 0);
            }
        }

        public virtual void BlockScheduledEvent(BlockDescriptor descriptor, IWorld world, object data)
        {
            // This space intentionally left blank
        }

        protected virtual ItemStack[] GetDrop(BlockDescriptor descriptor) // TODO: Include tools
        {
            short meta = 0;
            if (this is ICraftingRecipe)
                meta = (short)((this as ICraftingRecipe).SignificantMetadata ? descriptor.Metadata : 0);
            return new[] { new ItemStack(descriptor.ID, 1, meta) };
        }

        public virtual void ItemUsedOnEntity(ItemStack item, IEntity usedOn, IWorld world, IRemoteClient user)
        {
            // This space intentionally left blank
        }

        public virtual void ItemUsedOnNothing(ItemStack item, IWorld world, IRemoteClient user)
        {
            // This space intentionally left blank
        }

        public virtual void ItemUsedOnBlock(Coordinates3D coordinates, ItemStack item, BlockFace face, IWorld world, IRemoteClient user)
        {
            coordinates += MathHelper.BlockFaceToCoordinates(face);
            var old = world.GetBlockData(coordinates);
            byte[] overwritable =
            {
                AirBlock.BlockID,
                WaterBlock.BlockID,
                StationaryWaterBlock.BlockID,
                LavaBlock.BlockID,
                StationaryLavaBlock.BlockID
            };
            if (overwritable.Any(b => b == old.ID))
            {
                world.SetBlockID(coordinates, ID);
                world.SetMetadata(coordinates, (byte)item.Metadata);

                BlockPlaced(world.GetBlockData(coordinates), face, world, user);

                if (!IsSupported(world.GetBlockData(coordinates), user.Server, world))
                    world.SetBlockData(coordinates, old);
                else
                {
                    item.Count--;
                    user.Inventory[user.SelectedSlot] = item;
                }
            }
        }

        public virtual void BlockLoadedFromChunk(BlockDescriptor descriptor, IMultiplayerServer server, IWorld world)
        {
            // This space intentionally left blank
        }

        public virtual void TileEntityLoadedForClient(BlockDescriptor descriptor, IWorld world, NbtCompound entity, IRemoteClient client)
        {
            // This space intentionally left blank
        }

        short IItemProvider.ID
        {
            get
            {
                return ID;
            }
        }

        /// <summary>
        /// The ID of the block.
        /// </summary>
        public abstract byte ID { get; }

        public virtual Coordinates3D GetSupportDirection(BlockDescriptor descriptor)
        {
            return Coordinates3D.Zero;
        }

        /// <summary>
        /// The maximum amount that can be in a single stack of this block.
        /// </summary>
        public virtual sbyte MaximumStack { get { return 64; } }

        /// <summary>
        /// How resist the block is to explosions.
        /// </summary>
        public virtual double BlastResistance { get { return 0; } }

        /// <summary>
        /// How resist the block is to player mining/digging.
        /// </summary>
        public virtual double Hardness { get { return 0; } }

        /// <summary>
        /// The light level emitted by the block. 0 - 15
        /// </summary>
        public virtual byte Luminance { get { return 0; } }

        /// <summary>
        /// Whether or not the block is opaque
        /// </summary>
        public virtual bool Opaque { get { return true; } }

        /// <summary>
        /// Whether or not the block is rendered opaque
        /// </summary>
        public virtual bool RenderOpaque { get { return Opaque; } }

        /// <summary>
        /// The amount removed from the light level as it passes through this block.
        /// 255 - Let no light pass through(this may change)
        /// Notes:
        /// - This isn't needed for opaque blocks
        /// - This is needed since some "partial" transparent blocks remove more than 1 level from light passing through such as Ice.
        /// </summary>
        public virtual byte LightModifier { get { return 255; } }

        public virtual bool DiffuseSkyLight { get { return false; } }

        /// <summary>
        /// The name of the block as it would appear to players.
        /// </summary>
        public virtual string DisplayName { get { return string.Empty; } }

        public virtual Tuple<int, int> GetTextureMap(byte metadata)
        {
            return null;
        }

        public virtual BoundingBox? BoundingBox
        {
            get
            {
                return new BoundingBox(Vector3.Zero, Vector3.One);
            }
        }
    }
}