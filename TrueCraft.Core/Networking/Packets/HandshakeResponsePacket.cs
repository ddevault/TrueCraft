using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Sent from servers to continue with a connection. A kick is sent instead if the connection is refused.
    /// </summary>
    public struct HandshakeResponsePacket : IPacket
    {
        public byte ID { get { return 0x02; } }

        public HandshakeResponsePacket(string connectionHash)
        {
            ConnectionHash = connectionHash;
        }

        /// <summary>
        /// Set to "-" for offline mode servers. Online mode beta servers are obsolete.
        /// </summary>
        public string ConnectionHash;

        public void ReadPacket(IMinecraftStream stream)
        {
            ConnectionHash = stream.ReadString();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteString(ConnectionHash);
        }
    }
}