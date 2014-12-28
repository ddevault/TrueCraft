using System;
using TrueCraft.API.World;
using TrueCraft.API.Entities;

namespace TrueCraft.API.Networking
{
    public interface IRemoteClient
    {
        IMinecraftStream MinecraftStream { get; }
        bool DataAvailable { get; }
        IWorld World { get; }
        IEntity Entity { get; }

        void QueuePacket(IPacket packet);
    }
}