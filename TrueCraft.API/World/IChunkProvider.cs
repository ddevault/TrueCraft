using System;
using System.Collections.Generic;

namespace TrueCraft.API.World
{
    /// <summary>
    /// Provides new chunks to worlds. Generally speaking this is a terrain generator.
    /// </summary>
    public interface IChunkProvider
    {
        IList<IChunkDecorator> ChunkDecorators { get; }
        IChunk GenerateChunk(IWorld world, Coordinates2D coordinates);
        Coordinates3D GetSpawn(IWorld world);
    }
}