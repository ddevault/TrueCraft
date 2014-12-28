using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Sent by clients to inform the server of updates to their position and look direction.
    /// A combination of the PlayerPosition and PlayerLook packets.
    /// </summary>
    public struct PlayerPositionAndLookPacket : IPacket
    {
        public byte ID { get { return 0x0D; } }

        public PlayerPositionAndLookPacket(double x, double y, double stance, double z, float yaw, float pitch, bool onGround)
        {
            X = x;
            Y = y;
            Z = z;
            Stance = stance;
            Yaw = yaw;
            Pitch = pitch;
            OnGround = onGround;
        }

        public double X, Y, Z;
        public double Stance;
        public float Yaw, Pitch;
        public bool OnGround;

        public void ReadPacket(IMinecraftStream stream)
        {
            X = stream.ReadDouble();
            Y = stream.ReadDouble();
            Stance = stream.ReadDouble();
            Z = stream.ReadDouble();
            Yaw = stream.ReadSingle();
            Pitch = stream.ReadSingle();
            OnGround = stream.ReadBoolean();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteDouble(X);
            stream.WriteDouble(Y);
            stream.WriteDouble(Stance);
            stream.WriteDouble(Z);
            stream.WriteSingle(Yaw);
            stream.WriteSingle(Pitch);
            stream.WriteBoolean(OnGround);
        }
    }
}