using System;

namespace TrueCraft.API.Networking
{
    public interface IPacketReader
    {
        int ProtocolVersion { get; }

        void RegisterPacketType<T>(bool clientbound = true, bool serverbound = true) where T : IPacket;
        IPacket ReadPacket(IMinecraftStream stream, bool serverbound = true);
        void WritePacket(IMinecraftStream stream, IPacket packet);
    }
}