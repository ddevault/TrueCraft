using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Disconnects from a server or kicks a player. This is the last packet sent.
    /// </summary>
    public struct DisconnectPacket : IPacket
    {
        public byte ID { get { return 0xFF; } }

        public DisconnectPacket(string reason)
        {
            Reason = reason;
        }

        public string Reason;

        public void ReadPacket(IMinecraftStream stream)
        {
            Reason = stream.ReadString();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteString(Reason);
        }
    }
}