using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Updates the progress on the furnace UI.
    /// </summary>
    public struct UpdateProgressPacket : IPacket
    {
        public enum ProgressTarget
        {
            ItemCompletion = 0,
            AvailableHeat = 1,
        }

        public byte ID { get { return 0x69; } }

        public sbyte WindowID;
        public ProgressTarget Target;
        /// <summary>
        /// For the item completion, about 180 is full. For the available heat, about 250 is full.
        /// </summary>
        public short Value;

        public void ReadPacket(IMinecraftStream stream)
        {
            WindowID = stream.ReadInt8();
            Target = (ProgressTarget)stream.ReadInt16();
            Value = stream.ReadInt16();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteInt8(WindowID);
            stream.WriteInt16((short)Target);
            stream.WriteInt16(Value);
        }
    }
}