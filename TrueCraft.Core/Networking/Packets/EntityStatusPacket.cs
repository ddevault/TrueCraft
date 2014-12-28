using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Gives updates to what entities are doing.
    /// </summary>
    public struct EntityStatusPacket : IPacket
    {
        public enum EntityStatus
        {
            EntityHurt = 2,
            EntityKilled = 3,
            WolfTaming = 6,
            WolfTamed = 7,
            WolfShaking = 8,
            EatingAccepted = 9 // what
        }

        public byte ID { get { return 0x26; } }

        public int EntityID;
        public EntityStatus Status;

        public void ReadPacket(IMinecraftStream stream)
        {
            EntityID = stream.ReadInt32();
            Status = (EntityStatus)stream.ReadInt8();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteInt32(EntityID);
            stream.WriteInt8((sbyte)Status);
        }
    }
}