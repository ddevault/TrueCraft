using System;

namespace TrueCraft.Client.Events
{
    public class ChunkEventArgs : EventArgs
    {
        public ReadOnlyChunk Chunk { get; set; }

        public ChunkEventArgs(ReadOnlyChunk chunk)
        {
            Chunk = chunk;
        }
    }
}