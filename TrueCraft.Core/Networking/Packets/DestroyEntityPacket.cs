using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    public struct DestroyEntityPacket : IPacket
    {
        public byte ID { get { return 0x1D; } }

        public int EntityID;

        public void ReadPacket(IMinecraftStream stream)
        {
            EntityID = stream.ReadInt32();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteInt32(EntityID);
        }
    }
}