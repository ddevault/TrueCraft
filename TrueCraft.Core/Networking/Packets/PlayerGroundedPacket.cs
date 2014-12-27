using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Sent by clients to update whether or not the player is on the ground.
    /// Probably best to just ignore this.
    /// </summary>
    public struct PlayerGroundedPacket : IPacket
    {
        public byte ID { get { return 0x0A; }}

        public bool OnGround;

        public void ReadPacket(IMinecraftStream stream)
        {
            OnGround = stream.ReadBoolean();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteBoolean(OnGround);
        }
    }
}