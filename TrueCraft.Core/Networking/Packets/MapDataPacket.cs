using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    public struct MapDataPacket : IPacket
    {
        public byte ID { get { return 0x83; } }

        public short ItemID;
        public short Metadata;
        public byte[] Data;

        public void ReadPacket(IMinecraftStream stream)
        {
            ItemID = stream.ReadInt16();
            Metadata = stream.ReadInt16();
            byte length = stream.ReadUInt8();
            Data = stream.ReadUInt8Array(length);
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteInt16(ItemID);
            stream.WriteInt16(Metadata);
            stream.WriteUInt8((byte)Data.Length);
            stream.WriteUInt8Array(Data);
        }
    }
}