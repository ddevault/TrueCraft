using System;
using fNbt;
using System.Collections.Generic;

namespace TrueCraft.API.World
{
    public interface IChunk : IEventSubject, IDisposable, ISpatialBlockInformationProvider
    {
        bool IsModified { get; set; }
        bool LightPopulated { get; set; }
        DateTime LastAccessed { get; set; }
        byte[] Data { get; }
        bool TerrainPopulated { get; set; }
        Dictionary<Coordinates3D, NbtCompound> TileEntities { get; set; }
        NibbleSlice Metadata { get; }
        NibbleSlice BlockLight { get; }
        NibbleSlice SkyLight { get; }
        IRegion ParentRegion { get; set; }
        void UpdateHeightMap();
        byte GetSkyLight(Coordinates3D coordinates);
        byte GetBlockLight(Coordinates3D coordinates);
        void SetSkyLight(Coordinates3D coordinates, byte value);
        void SetBlockLight(Coordinates3D coordinates, byte value);
        NbtCompound GetTileEntity(Coordinates3D coordinates);
        void SetTileEntity(Coordinates3D coordinates, NbtCompound value);
    }
}