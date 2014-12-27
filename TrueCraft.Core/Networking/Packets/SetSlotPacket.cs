using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Sets the contents of an item slot on an inventory window.
    /// </summary>
    public struct SetSlotPacket : IPacket
    {
        public byte ID { get { return 0x67; } }

        public sbyte WindowID;
        public short SlotIndex;
        public short ItemID;
        public sbyte Count;
        public short Metadata;

        public void ReadPacket(IMinecraftStream stream)
        {
            WindowID = stream.ReadInt8();
            SlotIndex = stream.ReadInt16();
            ItemID = stream.ReadInt16();
            Count = stream.ReadInt8();
            Metadata = stream.ReadInt16();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteInt8(WindowID);
            stream.WriteInt16(SlotIndex);
            stream.WriteInt16(ItemID);
            stream.WriteInt8(Count);
            stream.WriteInt16(Metadata);
        }
    }
}