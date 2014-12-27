using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    public struct PlayerActionPacket : IPacket
    {
        public enum PlayerAction
        {
            Crouch = 1,
            Uncrouch = 2,
            LeaveBed = 3,
        }

        public byte ID { get { return 0x13; } }

        public int EntityID;
        public PlayerAction Action;

        public void ReadPacket(IMinecraftStream stream)
        {
            EntityID = stream.ReadInt32();
            Action = (PlayerAction)stream.ReadInt8();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteInt32(EntityID);
            stream.WriteInt8((sbyte)Action);
        }
    }
}