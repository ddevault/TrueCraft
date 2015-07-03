using System;
using TrueCraft.API.Networking;
using TrueCraft.API;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Does what it says on the tin.
    /// </summary>
    public struct SpawnMobPacket : IPacket
    {
        public byte ID { get { return 0x18; } }

        public SpawnMobPacket(int entityId, sbyte type, int x, int y, int z, sbyte yaw, sbyte pitch, MetadataDictionary metadata)
        {
            EntityID = entityId;
            MobType = type;
            X = x;
            Y = y;
            Z = z;
            Yaw = yaw;
            Pitch = pitch;
            Metadata = metadata;
        }

        public int EntityID;
        public sbyte MobType;
        public int X, Y, Z;
        public sbyte Yaw, Pitch;
        public MetadataDictionary Metadata;

        public void ReadPacket(IMinecraftStream stream)
        {
            EntityID = stream.ReadInt32();
            MobType = stream.ReadInt8();
            X = stream.ReadInt32();
            Y = stream.ReadInt32();
            Z = stream.ReadInt32();
            Yaw = stream.ReadInt8();
            Pitch = stream.ReadInt8();
            Metadata = MetadataDictionary.FromStream(stream);
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteInt32(EntityID);
            stream.WriteInt8(MobType);
            stream.WriteInt32(X);
            stream.WriteInt32(Y);
            stream.WriteInt32(Z);
            stream.WriteInt8(Yaw);
            stream.WriteInt8(Pitch);
            Metadata.WriteTo(stream);
        }
    }
}