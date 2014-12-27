using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Sent by clients when clicking on an inventory window.
    /// </summary>
    public struct ClickWindowPacket : IPacket
    {
        public byte ID { get { return 0x66; } }

        public sbyte WindowID;
        public short SlotIndex;
        public bool RightClick;
        public short TransactionID;
        public bool Shift;
        /// <summary>
        /// You should probably ignore this.
        /// </summary>
        public short ItemID;
        /// <summary>
        /// You should probably ignore this.
        /// </summary>
        public sbyte Count;
        /// <summary>
        /// You should probably ignore this.
        /// </summary>
        public sbyte Metadata;

        public void ReadPacket(IMinecraftStream stream)
        {
            WindowID = stream.ReadInt8();
            SlotIndex = stream.ReadInt16();
            RightClick = stream.ReadBoolean();
            TransactionID = stream.ReadInt16();
            Shift = stream.ReadBoolean();
            ItemID = stream.ReadInt16();
            Count = stream.ReadInt8();
            Metadata = stream.ReadInt8();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteInt8(WindowID);
            stream.WriteInt16(SlotIndex);
            stream.WriteBoolean(RightClick);
            stream.WriteInt16(TransactionID);
            stream.WriteBoolean(Shift);
            stream.WriteInt16(ItemID);
            stream.WriteInt8(Count);
            stream.WriteInt8(Metadata);
        }
    }
}