using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Sent by servers to show the animation of an item entity being collected by a player.
    /// </summary>
    public struct CollectItemPacket : IPacket
    {
        public byte ID { get { return 0x16; } }

        public int CollectedItemID;
        public int CollectorID;

        public void ReadPacket(IMinecraftStream stream)
        {
            CollectedItemID = stream.ReadInt32();
            CollectorID = stream.ReadInt32();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteInt32(CollectedItemID);
            stream.WriteInt32(CollectorID);
        }
    }
}