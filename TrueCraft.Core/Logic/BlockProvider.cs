using System;
using TrueCraft.API.Logic;
using TrueCraft.API.World;
using TrueCraft.API;
using TrueCraft.API.Networking;
using TrueCraft.Core.Entities;

namespace TrueCraft.Core.Logic
{
    /// <summary>
    /// Provides common implementations of block logic.
    /// </summary>
    public abstract class BlockProvider : IBlockProvider, IItemProvider
    {
        public virtual bool BlockRightClicked(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            return true;
        }

        public virtual bool BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            throw new NotImplementedException();
        }

        public virtual void BlockMined(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            var entityManager = user.Server.GetEntityManagerForWorld(world);
            var items = GetDrop(descriptor);
            foreach (var item in items)
            {
                entityManager.SpawnEntity(new ItemEntity(new Vector3(descriptor.Coordinates) + new Vector3(0.5), item));
            }
            world.SetBlockID(descriptor.Coordinates, 0);
        }

        public virtual void BlockUpdate(BlockDescriptor descriptor, IWorld world)
        {
            // This space intentionally left blank
        }

        public virtual void BlockScheduledEvent(BlockDescriptor descriptor, IWorld world, object data)
        {
            // This space intentionally left blank
        }

        protected virtual ItemStack[] GetDrop(BlockDescriptor descriptor) // TODO: Include tools
        {
            return new[] { new ItemStack(descriptor.ID, 1, descriptor.Metadata) };
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
        /// The amount removed from the light level as it passes through this block.
        /// 255 - Let no light pass through(this may change)
        /// Notes:
        /// - This isn't needed for opaque blocks
        /// - This is needed since some "partial" transparent blocks remove more than 1 level from light passing through such as Ice.
        /// </summary>
        public virtual byte LightModifier { get { return 1; } }

        /// <summary>
        /// The name of the block as it would appear to players.
        /// </summary>
        public virtual string DisplayName { get { return string.Empty; } }

        public virtual Tuple<int, int> GetTextureMap(byte metadata)
        {
            return null;
        }
    }
}