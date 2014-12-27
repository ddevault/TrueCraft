using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Sent from clients to begin a new connection.
    /// </summary>
    public struct HandshakePacket : IPacket
    {
        public byte ID { get { return 0x02; } }

        public HandshakePacket(string username)
        {
            Username = username;
        }

        public string Username;

        public void ReadPacket(IMinecraftStream stream)
        {
            Username = stream.ReadString();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteString(Username);
        }
    }
}