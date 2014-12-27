using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Sent periodically to confirm that the connection is still active. Send the same packet back
    /// to confirm it. Connection is dropped if no keep alive is received within one minute.
    /// </summary>
    public struct KeepAlivePacket : IPacket
    {
        public byte ID { get { return 0x00; } }

        public void ReadPacket(IMinecraftStream stream)
        {
            // This space intentionally left blank
        }

        public void WritePacket(IMinecraftStream stream)
        {
            // This space intentionally left blank
        }
    }
}