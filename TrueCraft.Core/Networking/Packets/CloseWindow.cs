using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Sent by the server to forcibly close an inventory window, or from the client when naturally closed.
    /// </summary>
    public struct CloseWindow : IPacket
    {
        public byte ID { get { return 0x65; } }

        public sbyte WindowID;

        public void ReadPacket(IMinecraftStream stream)
        {
            WindowID = stream.ReadInt8();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteInt8(WindowID);
        }
    }
}