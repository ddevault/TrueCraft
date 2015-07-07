using System;
using System.Collections.Generic;
using fNbt;
using fNbt.Serialization;
using TrueCraft.API.World;
using TrueCraft.API;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.World
{
    public class Chunk : INbtSerializable, IChunk
    {
        public const int Width = 16, Height = 128, Depth = 16;

        public event EventHandler Disposed;

        public void Dispose()
        {
            if (Disposed != null)
                Disposed(this, null);
        }

        [NbtIgnore]
        public DateTime LastAccessed { get; set; }
        [NbtIgnore]
        public bool IsModified { get; set; }
        [NbtIgnore]
        public byte[] Blocks { get; set; }
        [NbtIgnore]
        public NibbleArray Metadata { get; set; }
        [NbtIgnore]
        public NibbleArray BlockLight { get; set; }
        [NbtIgnore]
        public NibbleArray SkyLight { get; set; }
        public byte[] Biomes { get; set; }
        public int[] HeightMap { get; set; }
        public int MaxHeight { get; private set; }
        [TagName("xPos")]
        public int X { get; set; }
        [TagName("zPos")]
        public int Z { get; set; }
        [NbtIgnore]
        private bool _LightPopulated;
        public bool LightPopulated
        {
            get
            {
                return _LightPopulated;
            }
            set
            {
                _LightPopulated = value;
                IsModified = true;
            }
        }
        public Dictionary<Coordinates3D, NbtCompound> TileEntities { get; set; }

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
            Biomes = new byte[Width * Depth];
            HeightMap = new int[Width * Depth];
            TileEntities = new Dictionary<Coordinates3D, NbtCompound>();
            TerrainPopulated = false;
            LightPopulated = false;
            MaxHeight = 0;
        }

        public Chunk(Coordinates2D coordinates) : this()
        {
            X = coordinates.X;
            Z = coordinates.Z;
            const int size = Width * Height * Depth;
            Blocks = new byte[size];
            Metadata = new NibbleArray(size);
            BlockLight = new NibbleArray(size);
            SkyLight = new NibbleArray(size);
        }

        public byte GetBlockID(Coordinates3D coordinates)
        {
            int index = coordinates.Y + (coordinates.Z * Height) + (coordinates.X * Height * Width);
            return Blocks[index];
        }

        public byte GetMetadata(Coordinates3D coordinates)
        {
            int index = coordinates.Y + (coordinates.Z * Height) + (coordinates.X * Height * Width);
            return Metadata[index];
        }

        public byte GetSkyLight(Coordinates3D coordinates)
        {
            int index = coordinates.Y + (coordinates.Z * Height) + (coordinates.X * Height * Width);
            return SkyLight[index];
        }

        public byte GetBlockLight(Coordinates3D coordinates)
        {
            int index = coordinates.Y + (coordinates.Z * Height) + (coordinates.X * Height * Width);
            return BlockLight[index];
        }

        /// <summary>
        /// Sets the block ID at specific coordinates relative to this chunk.
        /// Warning: The parent world's BlockChanged event handler does not get called.
        /// </summary>
        public void SetBlockID(Coordinates3D coordinates, byte value)
        {
            IsModified = true;
            int index = coordinates.Y + (coordinates.Z * Height) + (coordinates.X * Height * Width);
            Blocks[index] = value;
            if (value == AirBlock.BlockID)
                Metadata[index] = 0x0;
            var oldHeight = GetHeight((byte)coordinates.X, (byte)coordinates.Z);
            if (value == AirBlock.BlockID)
            {
                if (oldHeight <= coordinates.Y)
                {
                    // Shift height downwards
                    while (coordinates.Y > 0)
                    {
                        coordinates.Y--;
                        if (GetBlockID(coordinates) != 0)
                        {
                            SetHeight((byte)coordinates.X, (byte)coordinates.Z, coordinates.Y);
                            if (coordinates.Y > MaxHeight)
                                MaxHeight = coordinates.Y;
                            break;
                        }
                    }
                }
            }
            else
            {
                if (oldHeight < coordinates.Y)
                    SetHeight((byte)coordinates.X, (byte)coordinates.Z, coordinates.Y);
            }
        }

        /// <summary>
        /// Sets the metadata at specific coordinates relative to this chunk.
        /// Warning: The parent world's BlockChanged event handler does not get called.
        /// </summary>
        public void SetMetadata(Coordinates3D coordinates, byte value)
        {
            IsModified = true;
            int index = coordinates.Y + (coordinates.Z * Height) + (coordinates.X * Height * Width);
            Metadata[index] = value;
        }

        /// <summary>
        /// Sets the sky light at specific coordinates relative to this chunk.
        /// Warning: The parent world's BlockChanged event handler does not get called.
        /// </summary>
        public void SetSkyLight(Coordinates3D coordinates, byte value)
        {
            IsModified = true;
            int index = coordinates.Y + (coordinates.Z * Height) + (coordinates.X * Height * Width);
            SkyLight[index] = value;
        }

        /// <summary>
        /// Sets the block light at specific coordinates relative to this chunk.
        /// Warning: The parent world's BlockChanged event handler does not get called.
        /// </summary>
        public void SetBlockLight(Coordinates3D coordinates, byte value)
        {
            IsModified = true;
            int index = coordinates.Y + (coordinates.Z * Height) + (coordinates.X * Height * Width);
            BlockLight[index] = value;
        }
        
        /// <summary>
        /// Gets the tile entity for the given coordinates. May return null.
        /// </summary>
        public NbtCompound GetTileEntity(Coordinates3D coordinates)
        {
            if (TileEntities.ContainsKey(coordinates))
                return TileEntities[coordinates];
            return null;
        }
        
        /// <summary>
        /// Sets the tile entity at the given coordinates to the given value.
        /// </summary>
        public void SetTileEntity(Coordinates3D coordinates, NbtCompound value)
        {
            if (value == null && TileEntities.ContainsKey(coordinates))
                TileEntities.Remove(coordinates);
            else
                TileEntities[coordinates] = value;
            IsModified = true;
        }

        /// <summary>
        /// Gets the height of the specified column.
        /// </summary>
        public int GetHeight(byte x, byte z)
        {
            return HeightMap[(x * Width) + z];
        }

        private void SetHeight(byte x, byte z, int value)
        {
            IsModified = true;
            HeightMap[(x * Width) + z] = value;
        }

        public void UpdateHeightMap()
        {
            for (byte x = 0; x < Chunk.Width; x++)
            {
                for (byte z = 0; z < Chunk.Depth; z++)
                {
                    int y;
                    for (y = Chunk.Height - 1; y >= 0; y--)
                    {
                        int index = y + (z * Height) + (x * Height * Width);
                        if (Blocks[index] != 0)
                        {
                            SetHeight(x, z, y);
                            if (y > MaxHeight)
                                MaxHeight = y;
                            break;
                        }
                    }
                    if (y == 0)
                        SetHeight(x, z, 0);
                }
            }
        }

        public NbtFile ToNbt()
        {
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
            var chunk = new NbtCompound(tagName);
            var entities = new NbtList("Entities", NbtTagType.Compound);
            chunk.Add(entities);
            chunk.Add(new NbtInt("X", X));
            chunk.Add(new NbtInt("Z", Z));
            chunk.Add(new NbtByte("LightPopulated", (byte)(LightPopulated ? 1 : 0)));
            chunk.Add(new NbtByte("TerrainPopulated", (byte)(TerrainPopulated ? 1 : 0)));
            chunk.Add(new NbtByteArray("Blocks", Blocks));
            chunk.Add(new NbtByteArray("Data", Metadata.Data));
            chunk.Add(new NbtByteArray("SkyLight", SkyLight.Data));
            chunk.Add(new NbtByteArray("BlockLight", BlockLight.Data));
            
            var tiles = new NbtList("TileEntities", NbtTagType.Compound);
            foreach (var kvp in TileEntities)
            {
                var c = new NbtCompound();
                c.Add(new NbtList("coordinates", new[] { 
                    new NbtInt(kvp.Key.X),
                    new NbtInt(kvp.Key.Y),
                    new NbtInt(kvp.Key.Z)
                 }));
                 c.Add(new NbtList("value", new[] { kvp.Value }));
                 tiles.Add(c);
            }
            chunk.Add(tiles);
            
            // TODO: Entities
            return chunk;
        }

        public void Deserialize(NbtTag value)
        {
            var tag = (NbtCompound)value;

            X = tag["X"].IntValue;
            Z = tag["Z"].IntValue;
            if (tag.Contains("TerrainPopulated"))
                TerrainPopulated = tag["TerrainPopulated"].ByteValue > 0;
            if (tag.Contains("LightPopulated"))
                LightPopulated = tag["LightPopulated"].ByteValue > 0;
            Blocks = tag["Blocks"].ByteArrayValue;
            Metadata = new NibbleArray();
            Metadata.Data = tag["Data"].ByteArrayValue;
            BlockLight = new NibbleArray();
            BlockLight.Data = tag["BlockLight"].ByteArrayValue;
            SkyLight = new NibbleArray();
            SkyLight.Data = tag["SkyLight"].ByteArrayValue;
            
            if (tag.Contains("TileEntities"))
            {
                foreach (var entity in tag["TileEntities"] as NbtList)
                {
                    TileEntities[new Coordinates3D(entity["coordinates"][0].IntValue,
                        entity["coordinates"][1].IntValue,
                        entity["coordinates"][2].IntValue)] = entity["value"][0] as NbtCompound;
                }
            }
            UpdateHeightMap();

            // TODO: Entities
        }
    }
}
