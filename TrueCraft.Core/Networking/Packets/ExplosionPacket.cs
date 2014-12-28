using System;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Networking.Packets
{
    public struct ExplosionPacket : IPacket
    {
        public byte ID { get { return 0x3C; } }

        public double X, Y, Z;
        public float Radius;
        public Tuple<sbyte, sbyte, sbyte>[] AffectedBlocks;

        public void ReadPacket(IMinecraftStream stream)
        {
            X = stream.ReadDouble();
            Y = stream.ReadDouble();
            Z = stream.ReadDouble();
            Radius = stream.ReadSingle();
            AffectedBlocks = new Tuple<sbyte, sbyte, sbyte>[stream.ReadInt32()];
            for (int i = 0; i < AffectedBlocks.Length; i++)
            {
                AffectedBlocks[i] = new Tuple<sbyte, sbyte, sbyte>(
                    stream.ReadInt8(),
                    stream.ReadInt8(),
                    stream.ReadInt8());
            }
        }

        public void WritePacket(IMinecraftStream stream)
        {
            stream.WriteDouble(X);
            stream.WriteDouble(Y);
            stream.WriteDouble(Z);
            stream.WriteSingle(Radius);
            stream.WriteInt32(AffectedBlocks.Length);
            for (int i = 0; i < AffectedBlocks.Length; i++)
            {
                stream.WriteInt8(AffectedBlocks[i].Item1);
                stream.WriteInt8(AffectedBlocks[i].Item2);
                stream.WriteInt8(AffectedBlocks[i].Item3);
            }
        }
    }
}

