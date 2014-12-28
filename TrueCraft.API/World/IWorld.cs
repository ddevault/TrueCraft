using System;

namespace TrueCraft.API.World
{
    // TODO: Entities
    /// <summary>
    /// An in-game world composed of chunks and blocks.
    /// </summary>
    public interface IWorld
    {
        string Name { get; set; }

        event EventHandler<BlockChangeEventArgs> BlockChanged;

        IChunk GetChunk(Coordinates2D coordinates);
        byte GetBlockID(Coordinates3D coordinates);
        byte GetMetadata(Coordinates3D coordinates);
        byte GetSkyLight(Coordinates3D coordinates);
        void SetBlockID(Coordinates3D coordinates, byte value);
        void SetMetadata(Coordinates3D coordinates, byte value);
        void SetSkyLight(Coordinates3D coordinates, byte value);
        void SetBlockLight(Coordinates3D coordinates, byte value);
        bool IsValidPosition(Coordinates3D position);
        void Save();
    }
}