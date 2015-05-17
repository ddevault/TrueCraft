using System;

namespace TrueCraft.Client.Events
{
    public class ChunkEventArgs
    {
        public ReadOnlyChunk Chunk { get; set; }

        public ChunkEventArgs(ReadOnlyChunk chunk)
        {
            Chunk = chunk;
        }
    }
}