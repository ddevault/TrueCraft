using System;

namespace TrueCraft.API.World
{
    public interface IChunk
    {
        Coordinates2D Coordinates { get; set; }
        bool IsModified { get; set; }
        int[] HeightMap { get; }
        ISection[] Sections { get; }
        byte[] Biomes { get; }
        DateTime LastAccessed { get; set; }
        short GetBlockID(Coordinates3D coordinates);
        byte GetMetadata(Coordinates3D coordinates);
        byte GetSkyLight(Coordinates3D coordinates);
        byte GetBlockLight(Coordinates3D coordinates);
        void SetBlockID(Coordinates3D coordinates, short value);
        void SetMetadata(Coordinates3D coordinates, byte value);
        void SetSkyLight(Coordinates3D coordinates, byte value);
        void SetBlockLight(Coordinates3D coordinates, byte value);
    }
}