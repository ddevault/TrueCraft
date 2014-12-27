using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Sent by servers to inform players of changes to the velocity of entities.
    /// Not sure exactly how all of that works, but I know it's going to be a pain in the ass.
    /// </summary>
    public struct EntityVelocityPacket : IPacket
    {
        public byte ID { get { return 0x1C; } }

        public int EntityID;
        public short XVelocity;
        public short YVelocity;
        public short ZVelocity;

        public void ReadPacket(IMinecraftStream stream)
        {
            EntityID = stream.ReadInt32();
            XVelocity = stream.ReadInt16();
            YVelocity = stream.ReadInt16();
            ZVelocity = stream.ReadInt16();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteInt32(EntityID);
            stream.WriteInt16(XVelocity);
            stream.WriteInt16(YVelocity);
            stream.WriteInt16(ZVelocity);
        }
    }
}