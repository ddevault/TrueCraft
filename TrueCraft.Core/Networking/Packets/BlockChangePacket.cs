using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    public struct BlockChangePacket : IPacket
    {
        public byte ID { get { return 0x35; } }

        public BlockChangePacket(int x, sbyte y, int z, sbyte blockID, sbyte metadata)
        {
            X = x;
            Y = y;
            Z = z;
            BlockID = blockID;
            Metadata = metadata;
        }

        public int X;
        public sbyte Y;
        public int Z;
        public sbyte BlockID;
        public sbyte Metadata;

        public void ReadPacket(IMinecraftStream stream)
        {
            X = stream.ReadInt32();
            Y = stream.ReadInt8();
            Z = stream.ReadInt32();
            BlockID = stream.ReadInt8();
            Metadata = stream.ReadInt8();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteInt32(X);
            stream.WriteInt8(Y);
            stream.WriteInt32(Z);
            stream.WriteInt8(BlockID);
            stream.WriteInt8(Metadata);
        }
    }
}