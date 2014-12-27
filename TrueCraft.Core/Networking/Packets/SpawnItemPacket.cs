using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Sent by servers to spawn item entities, I think.
    /// </summary>
    public struct SpawnItemPacket : IPacket
    {
        public byte ID { get { return 0x15; } }

        public int EntityID;
        public short ItemID;
        public sbyte Count;
        public short Metadata;
        public int X, Y, Z;
        public sbyte Yaw;
        public sbyte Pitch;
        public sbyte Roll;

        public void ReadPacket(IMinecraftStream stream)
        {
            EntityID = stream.ReadInt32();
            ItemID = stream.ReadInt16();
            Count = stream.ReadInt8();
            Metadata = stream.ReadInt16();
            X = stream.ReadInt32();
            Y = stream.ReadInt32();
            Z = stream.ReadInt32();
            Yaw = stream.ReadInt8();
            Pitch = stream.ReadInt8();
            Roll = stream.ReadInt8();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteInt32(EntityID);
            stream.WriteInt16(ItemID);
            stream.WriteInt8(Count);
            stream.WriteInt16(Metadata);
            stream.WriteInt32(X);
            stream.WriteInt32(Y);
            stream.WriteInt32(Z);
            stream.WriteInt8(Yaw);
            stream.WriteInt8(Pitch);
            stream.WriteInt8(Roll);
        }
    }
}