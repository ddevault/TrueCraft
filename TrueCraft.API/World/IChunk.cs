using System;
using fNbt;
using System.Collections.Generic;

namespace TrueCraft.API.World
{
    public interface IChunk
    {
        int X { get; }
        int Z { get; }
        Coordinates2D Coordinates { get; set; }
        bool IsModified { get; set; }
        int[] HeightMap { get; }
        byte[] Biomes { get; }
        DateTime LastAccessed { get; set; }
        byte[] Blocks { get; }
        Dictionary<Coordinates3D, NbtCompound> TileEntities { get; set; }
        NibbleArray Metadata { get; }
        NibbleArray BlockLight { get; }
        NibbleArray SkyLight { get; }
        int GetHeight(byte x, byte z);
        void UpdateHeightMap();
        byte GetBlockID(Coordinates3D coordinates);
        byte GetMetadata(Coordinates3D coordinates);
        byte GetSkyLight(Coordinates3D coordinates);
        byte GetBlockLight(Coordinates3D coordinates);
        void SetBlockID(Coordinates3D coordinates, byte value);
        void SetMetadata(Coordinates3D coordinates, byte value);
        void SetSkyLight(Coordinates3D coordinates, byte value);
        void SetBlockLight(Coordinates3D coordinates, byte value);
        NbtCompound GetTileEntity(Coordinates3D coordinates);
        void SetTileEntity(Coordinates3D coordinates, NbtCompound value);
    }
}