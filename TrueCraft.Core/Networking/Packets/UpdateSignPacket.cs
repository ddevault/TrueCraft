using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Sent by clients to update signs and by servers to notify clients of the change.
    /// </summary>
    public struct UpdateSignPacket : IPacket
    {
        public byte ID { get { return 0x82; } }

        public int X;
        public short Y;
        public int Z;
        public string[] Text;

        public void ReadPacket(IMinecraftStream stream)
        {
            X = stream.ReadInt32();
            Y = stream.ReadInt16();
            Z = stream.ReadInt32();
            Text = new string[4];
            Text[0] = stream.ReadString();
            Text[1] = stream.ReadString();
            Text[2] = stream.ReadString();
            Text[3] = stream.ReadString();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteInt32(X);
            stream.WriteInt32(Y);
            stream.WriteInt32(Z);
            stream.WriteString(Text[0]);
            stream.WriteString(Text[1]);
            stream.WriteString(Text[2]);
            stream.WriteString(Text[3]);
        }
    }
}