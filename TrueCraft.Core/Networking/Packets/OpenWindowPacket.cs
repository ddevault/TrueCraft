using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Instructs the client to open an inventory window.
    /// </summary>
    public struct OpenWindowPacket : IPacket
    {
        public byte ID { get { return 0x64; } }

        public OpenWindowPacket(sbyte windowID, sbyte type, string title, sbyte totalSlots)
        {
            WindowID = windowID;
            Type = type;
            Title = title;
            TotalSlots = totalSlots;
        }

        public sbyte WindowID;
        public sbyte Type;
        public string Title;
        public sbyte TotalSlots;

        public void ReadPacket(IMinecraftStream stream)
        {
            WindowID = stream.ReadInt8();
            Type = stream.ReadInt8();
            Title = stream.ReadString8();
            TotalSlots = stream.ReadInt8();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteInt8(WindowID);
            stream.WriteInt8(Type);
            stream.WriteString8(Title);
            stream.WriteInt8(TotalSlots);
        }
    }
}