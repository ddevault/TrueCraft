using System;

namespace TrueCraft.API.World
{
    public class ChunkGeneratedEventArgs : EventArgs
    {
        public Coordinates2D Coordinates { get; set; }
        public IChunk Chunk { get; set; }

        public ChunkGeneratedEventArgs(IChunk chunk)
        {
            Chunk = chunk;
            Coordinates = chunk.Coordinates;
        }
    }
}

