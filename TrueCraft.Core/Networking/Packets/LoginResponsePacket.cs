using System;
using TrueCraft.API.Networking;
using TrueCraft.API;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Sent by the server to allow the player to spawn, with information about the world being spawned into.
    /// </summary>
    public struct LoginResponsePacket : IPacket
    {
        public byte ID { get { return 0x01; } }

        public LoginResponsePacket(int entityID, long seed, Dimension dimension)
        {
            EntityID = entityID;
            Seed = seed;
            Dimension = dimension;
        }

        public int EntityID;
        public long Seed;
        public Dimension Dimension;

        public void ReadPacket(IMinecraftStream stream)
        {
            EntityID = stream.ReadInt32();
            stream.ReadString(); // Unused
            Seed = stream.ReadInt64();
            Dimension = (Dimension)stream.ReadInt8();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteInt32(EntityID);
            stream.WriteString(""); // Unused
            stream.WriteInt64(Seed);
            stream.WriteInt8((sbyte)Dimension);
        }
    }
}