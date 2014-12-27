using System;
using TrueCraft.API.Networking;
using TrueCraft.API;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Sent by clients when the player clicks "Respawn" after death. Sent by servers to confirm
    /// the respawn, and to respawn players in different dimensions (i.e. when using a portal).
    /// </summary>
    public struct RespawnPacket : IPacket
    {
        public byte ID { get { return 0x09; } }

        public Dimension Dimension;

        public void ReadPacket(IMinecraftStream stream)
        {
            Dimension = (Dimension)stream.ReadInt8();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteInt8((sbyte)Dimension);
        }
    }
}