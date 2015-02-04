using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    public struct SpawnPlayerPacket : IPacket
    {
        public byte ID { get { return 0x14; } }

        public int EntityID;
        public string PlayerName;
        public int X, Y, Z;
        public sbyte Yaw, Pitch;
        /// <summary>
        /// Note that this should be 0 for "no item".
        /// </summary>
        public short CurrentItem;

        public SpawnPlayerPacket(int entityID, string playerName, int x, int y, int z, sbyte yaw, sbyte pitch, short currentItem)
        {
            EntityID = entityID;
            PlayerName = playerName;
            X = x;
            Y = y;
            Z = z;
            Yaw = yaw;
            Pitch = pitch;
            CurrentItem = currentItem;
        }

        public void ReadPacket(IMinecraftStream stream)
        {
            EntityID = stream.ReadInt32();
            PlayerName = stream.ReadString();
            X = stream.ReadInt32();
            Y = stream.ReadInt32();
            Z = stream.ReadInt32();
            Yaw = stream.ReadInt8();
            Pitch = stream.ReadInt8();
            CurrentItem = stream.ReadInt16();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteInt32(EntityID);
            stream.WriteString(PlayerName);
            stream.WriteInt32(X);
            stream.WriteInt32(Y);
            stream.WriteInt32(Z);
            stream.WriteInt8(Yaw);
            stream.WriteInt8(Pitch);
            stream.WriteInt16(CurrentItem);
        }
    }
}