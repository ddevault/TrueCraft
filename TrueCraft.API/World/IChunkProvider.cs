using System;

namespace TrueCraft.API.World
{
    /// <summary>
    /// Provides new chunks to worlds. Generally speaking this is a terrain generator.
    /// </summary>
    public interface IChunkProvider
    {
        IChunk GenerateChunk(IWorld world, Coordinates2D coordinates);
    }
}