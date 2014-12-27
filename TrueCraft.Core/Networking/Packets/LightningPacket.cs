using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Spawns lightning at the given coordinates.
    /// </summary>
    public struct LightningPacket : IPacket
    {
        public byte ID { get { return 0x47; } }

        public int EntityID;
        public int X, Y, Z;

        public void ReadPacket(IMinecraftStream stream)
        {
            EntityID = stream.ReadInt32();
            stream.ReadBoolean(); // Unknown
            X = stream.ReadInt32();
            Y = stream.ReadInt32();
            Z = stream.ReadInt32();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteInt32(EntityID);
            stream.WriteBoolean(true); // Unknown
            stream.WriteInt32(X);
            stream.WriteInt32(Y);
            stream.WriteInt32(Z);
        }
    }
}