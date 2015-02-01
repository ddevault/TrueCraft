using System;

namespace TrueCraft.API.Logic
{
    public interface IItemProvider
    {
        short ID { get; }
        sbyte MaximumStack { get; }
        string DisplayName { get; }
        //bool ItemUsedOnBlock(ItemDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user);
    }
}