using fNbt;
using fNbt.Serialization;
using System;
using System.Collections.ObjectModel;

namespace TrueCraft.API
{
    /// <summary>
    /// Represents a slice of an array of 4-bit values.
    /// </summary>
    public class NibbleSlice : INbtSerializable
    {
        /// <summary>
        /// The data in the nibble array. Each byte contains
        /// two nibbles, stored in big-endian.
        /// </summary>
        public byte[] Data { get; private set; }
        public int Offset { get; private set; }
        public int Length { get; private set; }

        public NibbleSlice(byte[] data, int offset, int length)
        {
            Data = data;
            Offset = offset;
            Length = length;
        }

        /// <summary>
        /// Gets or sets a nibble at the given index.
        /// </summary>
        [NbtIgnore]
        public byte this[int index]
        {
            get { return (byte)(Data[Offset + index / 2] >> (index % 2 * 4) & 0xF); }
            set
            {
                value &= 0xF;
                Data[Offset + index / 2] &= (byte)(~(0xF << (index % 2 * 4)));
                Data[Offset + index / 2] |= (byte)(value << (index % 2 * 4));
            }
        }

        public byte[] ToArray()
        {
            byte[] array = new byte[Length];
            Buffer.BlockCopy(Data, Offset, array, 0, Length);
            return array;
        }

        public NbtTag Serialize(string tagName)
        {
            return new NbtByteArray(tagName, ToArray());
        }

        public void Deserialize(NbtTag value)
        {
            Length = value.ByteArrayValue.Length;
            Buffer.BlockCopy(value.ByteArrayValue, 0,
                Data, Offset, Length);
        }
    }

    public class ReadOnlyNibbleArray
    {
        private NibbleSlice NibbleArray { get; set; }

        public ReadOnlyNibbleArray(NibbleSlice array)
        {
            NibbleArray = array;
        }

        public byte this[int index]
        {
            get { return NibbleArray[index]; }
        }

        public ReadOnlyCollection<byte> Data
        {
            get { return Array.AsReadOnly(NibbleArray.Data); }
        }
    }
}