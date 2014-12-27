using System;

namespace TrueCraft.API.Networking
{
    public interface IPacket
    {
        byte ID { get; }
        void ReadPacket(IMinecraftStream stream);
        void WritePacket(IMinecraftStream stream);
    }
}