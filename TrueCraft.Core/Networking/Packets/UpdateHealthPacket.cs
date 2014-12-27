using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Sent by servers to inform clients of their current health.
    /// </summary>
    public struct UpdateHealthPacket : IPacket
    {
        public byte ID { get { return 0x08; } }

        public short Health;

        public void ReadPacket(IMinecraftStream stream)
        {
            Health = stream.ReadInt16();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteInt16(Health);
        }
    }
}