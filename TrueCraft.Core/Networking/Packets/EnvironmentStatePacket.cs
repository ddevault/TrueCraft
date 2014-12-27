using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Updates the player on changes to or status of the environment.
    /// </summary>
    public struct EnvironmentStatePacket : IPacket
    {
        public enum EnvironmentState
        {
            InvalidBed = 0,
            BeginRaining = 1,
            EndRaining = 2
        }

        public byte ID { get { return 0x46; } }

        public EnvironmentState State;

        public void ReadPacket(IMinecraftStream stream)
        {
            State = (EnvironmentState)stream.ReadInt8();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteInt8((sbyte)State);
        }
    }
}