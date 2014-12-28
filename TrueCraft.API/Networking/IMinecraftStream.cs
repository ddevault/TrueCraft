using System;
using System.IO;

namespace TrueCraft.API.Networking
{
    public interface IMinecraftStream
    {
        Stream BaseStream { get; }

        byte ReadUInt8();
        sbyte ReadInt8();
        void WriteUInt8(byte value);
        void WriteInt8(sbyte value);

        ushort ReadUInt16();
        short ReadInt16();
        void WriteUInt16(ushort value);
        void WriteInt16(short value);

        uint ReadUInt32();
        int ReadInt32();
        void WriteUInt32(uint value);
        void WriteInt32(int value);

        ulong ReadUInt64();
        long ReadInt64();
        void WriteUInt64(ulong value);
        void WriteInt64(long value);

        float ReadSingle();
        void WriteSingle(float value);

        double ReadDouble();
        void WriteDouble(double value);

        string ReadString();
        void WriteString(string value);
        string ReadString8();
        void WriteString8(string value);

        bool ReadBoolean();
        void WriteBoolean(bool value);

        byte[] ReadUInt8Array(int length);
        void WriteUInt8Array(byte[] value);
        sbyte[] ReadInt8Array(int length);
        void WriteInt8Array(sbyte[] value);

        ushort[] ReadUInt16Array(int length);
        void WriteUInt16Array(ushort[] value);
        short[] ReadInt16Array(int length);
        void WriteInt16Array(short[] value);

        uint[] ReadUInt32Array(int length);
        void WriteUInt32Array(uint[] value);
        int[] ReadInt32Array(int length);
        void WriteInt32Array(int[] value);

        ulong[] ReadUInt64Array(int length);
        void WriteUInt64Array(ulong[] value);
        long[] ReadInt64Array(int length);
        void WriteInt64Array(long[] value);
    }
}