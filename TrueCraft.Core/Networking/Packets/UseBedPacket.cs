using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Sent by servers to indicate that a player is using a bed. More research required on this packet.
    /// </summary>
    public struct UseBedPacket : IPacket
    {
        public byte ID { get { return 0x11; } }

        public int EntityID;
        public bool InBed;
        public int X;
        public sbyte Y;
        public int Z;

        public void ReadPacket(IMinecraftStream stream)
        {
            EntityID = stream.ReadInt32();
            InBed = stream.ReadBoolean();
            X = stream.ReadInt32();
            Y = stream.ReadInt8();
            Z = stream.ReadInt32();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteInt32(EntityID);
            stream.WriteBoolean(InBed);
            stream.WriteInt32(X);
            stream.WriteInt8(Y);
            stream.WriteInt32(Z);
        }
    }
}