using System;

namespace TrueCraft.Client.Linux.Events
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