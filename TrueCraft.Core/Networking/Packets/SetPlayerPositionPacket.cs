using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Sent by servers to set the position and look of the player. Can be used to teleport players.
    /// </summary>
    public struct SetPlayerPositionPacket : IPacket
    {
        public byte ID { get { return 0x0D; } }

        public double X, Y, Z;
        public double Stance;
        public float Yaw, Pitch;
        public bool OnGround;

        public void ReadPacket(IMinecraftStream stream)
        {
            X = stream.ReadDouble();
            Stance = stream.ReadDouble();
            Y = stream.ReadDouble();
            Z = stream.ReadDouble();
            Yaw = stream.ReadSingle();
            Pitch = stream.ReadSingle();
            OnGround = stream.ReadBoolean();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteDouble(X);
            stream.WriteDouble(Stance);
            stream.WriteDouble(Y);
            stream.WriteDouble(Z);
            stream.WriteDouble(Yaw);
            stream.WriteDouble(Pitch);
            stream.WriteBoolean(OnGround);
        }
    }
}