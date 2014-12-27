using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Sent by clients upon clicking an entity.
    /// </summary>
    public struct UseEntityPacket : IPacket
    {
        public byte ID { get { return 0x07; } }

        public int UserID;
        public int TargetID;
        public bool LeftClick;

        public void ReadPacket(IMinecraftStream stream)
        {
            UserID = stream.ReadInt32();
            TargetID = stream.ReadInt32();
            LeftClick = stream.ReadBoolean();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteInt32(UserID);
            stream.WriteInt32(TargetID);
            stream.WriteBoolean(LeftClick);
        }
    }
}