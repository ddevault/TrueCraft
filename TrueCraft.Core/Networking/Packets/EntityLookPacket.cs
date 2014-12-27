using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Sent by servers to update the direction an entity is looking in.
    /// </summary>
    public struct EntityLookPacket : IPacket
    {
        public byte ID { get { return 0x20; } }

        public int EntityID;
        public sbyte Yaw, Pitch;

        public void ReadPacket(IMinecraftStream stream)
        {
            EntityID = stream.ReadInt32();
            Yaw = stream.ReadInt8();
            Pitch = stream.ReadInt8();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteInt32(EntityID);
            stream.WriteInt8(Yaw);
            stream.WriteInt8(Pitch);
        }
    }
}