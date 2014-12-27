using System;

namespace TrueCraft.API.Networking
{
    public interface IRemoteClient
    {
        IMinecraftStream MinecraftStream { get; }
        bool DataAvailable { get; }

        void QueuePacket(IPacket packet);
    }
}