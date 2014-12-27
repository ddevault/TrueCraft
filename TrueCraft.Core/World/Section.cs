using fNbt.Serialization;
using TrueCraft.API;
using TrueCraft.API.World;

namespace TrueCraft.Core.World
{
    public class Section : ISection
    {
        public const byte Width = 16, Height = 16, Depth = 16;

        public byte[] Blocks { get; set; }
        [TagName("Data")]
        public NibbleArray Metadata { get; set; }
        public NibbleArray BlockLight { get; set; }
        public NibbleArray SkyLight { get; set; }
        [IgnoreOnNull]
        public NibbleArray Add { get; set; }
        public byte Y { get; set; }

        private int nonAirCount;

        public Section()
        {
        }

        public Section(byte y)
        {
            const int size = Width * Height * Depth;
            this.Y = y;
            Blocks = new byte[size];
            Metadata = new NibbleArray(size);
            BlockLight = new NibbleArray(size);
            SkyLight = new NibbleArray(size);
            for (int i = 0; i < size; i++)
                SkyLight[i] = 0xFF;
            Add = null; // Only used when needed
            nonAirCount = 0;
        }

        [NbtIgnore]
        public bool IsAir
        {
            get { return nonAirCount == 0; }
        }

        public short GetBlockID(Coordinates3D coordinates)
        {
            int index = coordinates.X + (coordinates.Z * Width) + (coordinates.Y * Height * Width);
            short value = Blocks[index];
            if (Add != null)
                value |= (short)(Add[index] << 8);
            return value;
        }

        public byte GetMetadata(Coordinates3D coordinates)
        {
            int index = coordinates.X + (coordinates.Z * Width) + (coordinates.Y * Height * Width);
            return Metadata[index];
        }

        public byte GetSkyLight(Coordinates3D coordinates)
        {
            int index = coordinates.X + (coordinates.Z * Width) + (coordinates.Y * Height * Width);
            return SkyLight[index];
        }

        public byte GetBlockLight(Coordinates3D coordinates)
        {
            int index = coordinates.X + (coordinates.Z * Width) + (coordinates.Y * Height * Width);
            return BlockLight[index];
        }

        public void SetBlockID(Coordinates3D coordinates, short value)
        {
            int index = coordinates.X + (coordinates.Z * Width) + (coordinates.Y * Height * Width);
            if (value == 0)
            {
                if (Blocks[index] != 0)
                    nonAirCount--;
            }
            else
            {
                if (Blocks[index] == 0)
                    nonAirCount++;
            }
            Blocks[index] = (byte)value;
            if ((value & ~0xFF) != 0)
            {
                if (Add == null) Add = new NibbleArray(Width * Height * Depth);
                Add[index] = (byte)((ushort)value >> 8);
            }
        }

        public void SetMetadata(Coordinates3D coordinates, byte value)
        {
            int index = coordinates.X + (coordinates.Z * Width) + (coordinates.Y * Height * Width);
            Metadata[index] = value;
        }

        public void SetSkyLight(Coordinates3D coordinates, byte value)
        {
            int index = coordinates.X + (coordinates.Z * Width) + (coordinates.Y * Height * Width);
            SkyLight[index] = value;
        }

        public void SetBlockLight(Coordinates3D coordinates, byte value)
        {
            int index = coordinates.X + (coordinates.Z * Width) + (coordinates.Y * Height * Width);
            BlockLight[index] = value;
        }

        public void ProcessSection()
        {
            // TODO: Schedule updates
            nonAirCount = 0;
            for (int i = 0; i < Blocks.Length; i++)
            {
                if (Blocks[i] != 0)
                    nonAirCount++;
            }
        }
    }
}