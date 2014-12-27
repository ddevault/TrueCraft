using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Reflection;
using fNbt;
using fNbt.Serialization;
using TrueCraft.API.World;
using TrueCraft.API;

namespace TrueCraft.Core.World
{
    public class Chunk : INbtSerializable, IChunk
    {
        public const int Width = 16, Height = 256, Depth = 16;

        private static readonly NbtSerializer Serializer = new NbtSerializer(typeof(Chunk));

        [NbtIgnore]
        public DateTime LastAccessed { get; set; }

        public bool IsModified { get; set; }

        public byte[] Biomes { get; set; }

        public int[] HeightMap { get; set; }

        [NbtIgnore]
        public ISection[] Sections { get; set; }

        [TagName("xPos")]
        public int X { get; set; }

        [TagName("zPos")]
        public int Z { get; set; }

        public Coordinates2D Coordinates
        {
            get
            {
                return new Coordinates2D(X, Z);
            }
            set
            {
                X = value.X;
                Z = value.Z;
            }
        }

        public long LastUpdate { get; set; }

        public bool TerrainPopulated { get; set; }

        [NbtIgnore]
        public Region ParentRegion { get; set; }

        public Chunk()
        {
            TerrainPopulated = true;
            Sections = new Section[16];
            for (int i = 0; i < Sections.Length; i++)
                Sections[i] = new Section((byte)i);
            Biomes = new byte[Width * Depth];
            HeightMap = new int[Width * Depth];
            LastAccessed = DateTime.Now;
        }

        public Chunk(Coordinates2D coordinates) : this()
        {
            X = coordinates.X;
            Z = coordinates.Z;
        }

        public byte GetBlockID(Coordinates3D coordinates)
        {
            LastAccessed = DateTime.Now;
            int section = GetSectionNumber(coordinates.Y);
            coordinates.Y = GetPositionInSection(coordinates.Y);
            return Sections[section].GetBlockID(coordinates);
        }

        public byte GetMetadata(Coordinates3D coordinates)
        {
            LastAccessed = DateTime.Now;
            int section = GetSectionNumber(coordinates.Y);
            coordinates.Y = GetPositionInSection(coordinates.Y);
            return Sections[section].GetMetadata(coordinates);
        }

        public byte GetSkyLight(Coordinates3D coordinates)
        {
            LastAccessed = DateTime.Now;
            int section = GetSectionNumber(coordinates.Y);
            coordinates.Y = GetPositionInSection(coordinates.Y);
            return Sections[section].GetSkyLight(coordinates);
        }

        public byte GetBlockLight(Coordinates3D coordinates)
        {
            LastAccessed = DateTime.Now;
            int section = GetSectionNumber(coordinates.Y);
            coordinates.Y = GetPositionInSection(coordinates.Y);
            return Sections[section].GetBlockLight(coordinates);
        }

        public void SetBlockID(Coordinates3D coordinates, byte value)
        {
            LastAccessed = DateTime.Now;
            IsModified = true;
            int section = GetSectionNumber(coordinates.Y);
            coordinates.Y = GetPositionInSection(coordinates.Y);
            Sections[section].SetBlockID(coordinates, value);
            var oldHeight = GetHeight((byte)coordinates.X, (byte)coordinates.Z);
            if (value == 0) // Air
            {
                if (oldHeight <= coordinates.Y)
                {
                    // Shift height downwards
                    while (coordinates.Y > 0)
                    {
                        coordinates.Y--;
                        if (GetBlockID(coordinates) != 0)
                            SetHeight((byte)coordinates.X, (byte)coordinates.Z, coordinates.Y);
                    }
                }
            }
            else
            {
                if (oldHeight < coordinates.Y)
                    SetHeight((byte)coordinates.X, (byte)coordinates.Z, coordinates.Y);
            }
        }

        public void SetMetadata(Coordinates3D coordinates, byte value)
        {
            LastAccessed = DateTime.Now;
            IsModified = true;
            int section = GetSectionNumber(coordinates.Y);
            coordinates.Y = GetPositionInSection(coordinates.Y);
            Sections[section].SetMetadata(coordinates, value);
        }

        public void SetSkyLight(Coordinates3D coordinates, byte value)
        {
            LastAccessed = DateTime.Now;
            IsModified = true;
            int section = GetSectionNumber(coordinates.Y);
            coordinates.Y = GetPositionInSection(coordinates.Y);
            Sections[section].SetSkyLight(coordinates, value);
        }

        public void SetBlockLight(Coordinates3D coordinates, byte value)
        {
            LastAccessed = DateTime.Now;
            IsModified = true;
            int section = GetSectionNumber(coordinates.Y);
            coordinates.Y = GetPositionInSection(coordinates.Y);
            Sections[section].SetBlockLight(coordinates, value);
        }

        private static int GetSectionNumber(int yPos)
        {
             return yPos / 16;
        }

        private static int GetPositionInSection(int yPos)
        {
            return yPos % 16;
        }

        /// <summary>
        /// Gets the height of the specified column.
        /// </summary>
        public int GetHeight(byte x, byte z)
        {
            LastAccessed = DateTime.Now;
            return HeightMap[(byte)(z * Depth) + x];
        }

        private void SetHeight(byte x, byte z, int value)
        {
            LastAccessed = DateTime.Now;
            IsModified = true;
            HeightMap[(byte)(z * Depth) + x] = value;
        }

        public NbtFile ToNbt()
        {
            LastAccessed = DateTime.Now;
            var serializer = new NbtSerializer(typeof(Chunk));
            var compound = serializer.Serialize(this, "Level") as NbtCompound;
            var file = new NbtFile();
            file.RootTag.Add(compound);
            return file;
        }

        public static Chunk FromNbt(NbtFile nbt)
        {
            var serializer = new NbtSerializer(typeof(Chunk));
            var chunk = (Chunk)serializer.Deserialize(nbt.RootTag["Level"]);
            return chunk;
        }

        public NbtTag Serialize(string tagName)
        {
            var chunk = (NbtCompound)Serializer.Serialize(this, tagName, true);
            var entities = new NbtList("Entities", NbtTagType.Compound);
            chunk.Add(entities);
            var sections = new NbtList("Sections", NbtTagType.Compound);
            var serializer = new NbtSerializer(typeof(Section));
            for (int i = 0; i < Sections.Length; i++)
            {
                if (Sections[i] is Section)
                {
                    if (!(Sections[i] as Section).IsAir)
                        sections.Add(serializer.Serialize(Sections[i]));
                }
                else
                    sections.Add(serializer.Serialize(Sections[i]));
            }
            chunk.Add(sections);
            return chunk;
        }

        public void Deserialize(NbtTag value)
        {
            IsModified = true;
            var compound = value as NbtCompound;
            var chunk = (Chunk)Serializer.Deserialize(value, true);

            this.Biomes = chunk.Biomes;
            this.HeightMap = chunk.HeightMap;
            this.LastUpdate = chunk.LastUpdate;
            this.Sections = chunk.Sections;
            this.TerrainPopulated = chunk.TerrainPopulated;
            this.X = chunk.X;
            this.Z = chunk.Z;

            var serializer = new NbtSerializer(typeof(Section));
            foreach (var section in compound["Sections"] as NbtList)
            {
                int index = section["Y"].IntValue;
                Sections[index] = (Section)serializer.Deserialize(section);
                Sections[index].ProcessSection();
            }
        }
    }
}
