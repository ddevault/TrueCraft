using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Spawns entities that don't fit into any other bucket of entities.
    /// </summary>
    public struct SpawnGenericEntityPacket : IPacket
    {
        public byte ID { get { return 0x17; } }

        public SpawnGenericEntityPacket(int entityID, sbyte entityType, int x, int y, int z,
            int data, short? xVelocity, short? yVelocity, short? zVelocity)
        {
            EntityID = entityID;
            EntityType = entityType;
            X = x;
            Y = y;
            Z = z;
            Data = data;
            XVelocity = xVelocity;
            YVelocity = yVelocity;
            ZVelocity = zVelocity;
        }

        public int EntityID;
        public sbyte EntityType; // TODO: Enum? Maybe a lookup would be better.
        public int X, Y, Z;
        public int Data;
        public short? XVelocity, YVelocity, ZVelocity;

        public void ReadPacket(IMinecraftStream stream)
        {
            EntityID = stream.ReadInt32();
            EntityType = stream.ReadInt8();
            X = stream.ReadInt32();
            Y = stream.ReadInt32();
            Z = stream.ReadInt32();
            Data = stream.ReadInt32();
            if (Data > 0)
            {
                XVelocity = stream.ReadInt16();
                YVelocity = stream.ReadInt16();
                ZVelocity = stream.ReadInt16();
            }
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteInt32(EntityID);
            stream.WriteInt8(EntityType);
            stream.WriteInt32(X);
            stream.WriteInt32(Y);
            stream.WriteInt32(Z);
            stream.WriteInt32(Data);
            if (Data > 0)
            {
                stream.WriteInt16(XVelocity.Value);
                stream.WriteInt16(YVelocity.Value);
                stream.WriteInt16(ZVelocity.Value);
            }
        }
    }
}