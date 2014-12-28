using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Sends actual blocks to populate chunks with.
    /// </summary>
    public struct ChunkDataPacket : IPacket
    {
        public byte ID { get { return 0x33; } }

        public ChunkDataPacket(int x, short y, int z, short width, short height, short depth, byte[] compressedData)
        {
            X = x;
            Y = y;
            Z = z;
            Width = width;
            Height = height;
            Depth = depth;
            CompressedData = compressedData;
        }

        public int X;
        public short Y;
        public int Z;
        public short Width, Height, Depth;
        public byte[] CompressedData;

        public void ReadPacket(IMinecraftStream stream)
        {
            X = stream.ReadInt32();
            Y = stream.ReadInt16();
            Z = stream.ReadInt32();
            Width = (short)(stream.ReadInt8() + 1);
            Height = (short)(stream.ReadInt8() + 1);
            Depth = (short)(stream.ReadInt8() + 1);
            int len = stream.ReadInt32();
            CompressedData = stream.ReadUInt8Array(len);
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteInt32(X);
            stream.WriteInt16(Y);
            stream.WriteInt32(Z);
            stream.WriteInt8((sbyte)(Width - 1));
            stream.WriteInt8((sbyte)(Height - 1));
            stream.WriteInt8((sbyte)(Depth - 1));
            stream.WriteInt32(CompressedData.Length);
            stream.WriteUInt8Array(CompressedData);
        }
    }
}