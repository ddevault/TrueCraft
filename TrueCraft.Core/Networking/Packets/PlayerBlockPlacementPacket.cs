using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Sent when the player interacts with a block (generally via right clicking).
    /// This is also used for items that don't interact with blocks (i.e. food) with the coordinates set to -1.
    /// </summary>
    public struct PlayerBlockPlacementPacket : IPacket
    {
        public byte ID { get { return 0x0F; } }

        public int X;
        public sbyte Y;
        public int Z;
        public NetworkBlockFace Direction;
        /// <summary>
        /// The block or item ID. You should probably ignore this and use a server-side inventory.
        /// </summary>
        public short ItemID;
        /// <summary>
        /// The amount in the player's hand. Who cares?
        /// </summary>
        public sbyte? Amount;
        /// <summary>
        /// The block metadata. You should probably ignore this and use a server-side inventory.
        /// </summary>
        public short? Metadata;

        public void ReadPacket(IMinecraftStream stream)
        {
            X = stream.ReadInt32();
            Y = stream.ReadInt8();
            Z = stream.ReadInt32();
            Direction = (NetworkBlockFace)stream.ReadInt8();
            ItemID = stream.ReadInt16();
            if (ItemID != 0)
            {
                Amount = stream.ReadInt8();
                Metadata = stream.ReadInt16();
            }
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteInt32(X);
            stream.WriteInt8(Y);
            stream.WriteInt32(Z);
            stream.WriteInt8((sbyte)Direction);
            stream.WriteInt16(ItemID);
            if (ItemID != 0)
            {
                stream.WriteInt8(Amount.Value);
                stream.WriteInt16(Metadata.Value);
            }
        }
    }
}