using System;
using TrueCraft.API.World;
using TrueCraft.API.Networking;
using TrueCraft.API.Entities;

namespace TrueCraft.API.Logic
{
    public interface IItemProvider
    {
        short ID { get; }
        sbyte MaximumStack { get; }
        string DisplayName { get; }
        void ItemUsedOnNothing(ItemStack item, IWorld world, IRemoteClient user);
        void ItemUsedOnEntity(ItemStack item, IEntity usedOn, IWorld world, IRemoteClient user);
        void ItemUsedOnBlock(Coordinates3D coordinates, ItemStack item, BlockFace face, IWorld world, IRemoteClient user);
        Tuple<int, int> GetIconTexture(byte metadata);
    }
}