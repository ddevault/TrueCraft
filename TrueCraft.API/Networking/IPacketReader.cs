using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace TrueCraft.API.Networking
{
    public interface IPacketReader
    {
        int ProtocolVersion { get; }

        ConcurrentDictionary<object, IPacketSegmentProcessor> Processors { get; }
        void RegisterPacketType<T>(bool clientbound = true, bool serverbound = true) where T : IPacket;
        IEnumerable<IPacket> ReadPackets(object key, byte[] buffer, int offset, int length, bool serverbound = true);
        void WritePacket(IMinecraftStream stream, IPacket packet);
    }
}