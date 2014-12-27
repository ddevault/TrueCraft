using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Used to allocate or unload chunks.
    /// </summary>
    public struct ChunkPreamblePacket : IPacket
    {
        public byte ID { get { return 0x32; } }

        public int X, Z;
        /// <summary>
        /// If false, free the chunk. If true, allocate it.
        /// </summary>
        public bool Load;

        public void ReadPacket(IMinecraftStream stream)
        {
            X = stream.ReadInt32();
            Z = stream.ReadInt32();
            Load = stream.ReadBoolean();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteInt32(X);
            stream.WriteInt32(Z);
            stream.WriteBoolean(Load);
        }
    }
}