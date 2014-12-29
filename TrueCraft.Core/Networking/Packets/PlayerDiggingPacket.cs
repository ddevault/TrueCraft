using System;
using TrueCraft.API.Networking;
using TrueCraft.API;

namespace TrueCraft.Core.Networking.Packets
{
    /// <summary>
    /// Sent by clients when they start or stop digging. Also used to throw items.
    /// Also sent with action set to StartDigging when clicking to open a door.
    /// </summary>
    public struct PlayerDiggingPacket : IPacket
    {
        public enum Action
        {
            StartDigging = 0,
            StopDigging = 2,
            DropItem = 4
        }
        
        public byte ID { get { return 0x0E; } }

        public Action PlayerAction;
        public int X;
        public sbyte Y;
        public int Z;
        public BlockFace Face;

        public void ReadPacket(IMinecraftStream stream)
        {
            PlayerAction = (Action)stream.ReadInt8();
            X = stream.ReadInt32();
            Y = stream.ReadInt8();
            Z = stream.ReadInt32();
            Face = (BlockFace)stream.ReadInt8();
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteInt8((sbyte)PlayerAction);
            stream.WriteInt32(X);
            stream.WriteInt8(Y);
            stream.WriteInt32(Z);
            stream.WriteInt8((sbyte)Face);
        }
    }
}