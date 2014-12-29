using System;
using TrueCraft.API.World;
using TrueCraft.API.Entities;
using TrueCraft.API.Windows;

namespace TrueCraft.API.Networking
{
    public interface IRemoteClient
    {
        IMinecraftStream MinecraftStream { get; }
        bool DataAvailable { get; }
        IWorld World { get; }
        IEntity Entity { get; }
        IWindow Inventory { get; }
        string Username { get; }

        void QueuePacket(IPacket packet);
        void SendMessage(string message);
    }
}