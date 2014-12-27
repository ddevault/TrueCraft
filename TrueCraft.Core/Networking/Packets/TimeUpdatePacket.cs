using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Sent from the server to inform the client of the in-game time of day.
    /// The number sent is in 1/20th of a second increments, where 24000 ticks are in each day.
    /// </summary>
    public struct TimeUpdatePacket : IPacket
    {
        public byte ID { get { return 0x04; } }

        public TimeUpdatePacket(long time)
        {
            Time = time;
        }

        public long Time;

        public void ReadPacket(IMinecraftStream stream)
        {
            Time = stream.ReadInt64();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteInt64(Time);
        }
    }
}