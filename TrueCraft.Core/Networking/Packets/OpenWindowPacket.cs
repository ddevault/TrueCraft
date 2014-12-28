using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Instructs the client to open an inventory window.
    /// </summary>
    public struct OpenWindowPacket : IPacket
    {
        public enum WindowType
        {
            Chest = 0,
            Workbench = 1,
            Furnace = 2,
            Dispenser = 3
        }

        public byte ID { get { return 0x64; } }

        public sbyte WindowID;
        public WindowType Type;
        public string Title;
        public sbyte TotalSlots;

        public void ReadPacket(IMinecraftStream stream)
        {
            WindowID = stream.ReadInt8();
            Type = (WindowType)stream.ReadInt8();
            Title = stream.ReadString8();
            TotalSlots = stream.ReadInt8();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteInt8(WindowID);
            stream.WriteInt8((sbyte)Type);
            stream.WriteString8(Title);
            stream.WriteInt8(TotalSlots);
        }
    }
}