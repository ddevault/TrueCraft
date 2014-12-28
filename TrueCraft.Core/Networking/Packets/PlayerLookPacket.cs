using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Sent to update the rotation of the player's head and body.
    /// </summary>
    public struct PlayerLookPacket : IPacket
    {
        public byte ID { get { return 0x0C; } }

        public float Yaw, Pitch;
        public bool OnGround;

        public void ReadPacket(IMinecraftStream stream)
        {
            Yaw = stream.ReadSingle();
            Pitch = stream.ReadSingle();
            OnGround = stream.ReadBoolean();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteSingle(Yaw);
            stream.WriteSingle(Pitch);
            stream.WriteBoolean(OnGround);
        }
    }
}