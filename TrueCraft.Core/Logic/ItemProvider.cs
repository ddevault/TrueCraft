using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.API.Entities;
using TrueCraft.API.World;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Logic
{
    public abstract class ItemProvider : IItemProvider
    {
        public abstract short ID { get; }

        public virtual sbyte MaximumStack { get { return 64; } }

        public virtual string DisplayName { get { return string.Empty; } }

        public virtual void ItemUsedOnEntity(ItemStack item, IEntity usedOn, IWorld world, IRemoteClient user)
        {
            // This space intentionally left blank
        }

        public virtual void ItemUsedOnBlock(Coordinates3D coordinates, ItemStack item, BlockFace face, IWorld world, IRemoteClient user)
        {
            // This space intentionally left blank
        }

        public virtual void ItemUsedOnNothing(ItemStack item, IWorld world, IRemoteClient user)
        {
            // This space intentionally left blank
        }
    }
}