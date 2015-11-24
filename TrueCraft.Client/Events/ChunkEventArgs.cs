using System;
using TrueCraft.API;

namespace TrueCraft.Client.Events
{
    public class ChunkEventArgs : EventArgs
    {
        public ReadOnlyChunk Chunk { get; set; }
        public Coordinates3D Source { get; set; }

        public ChunkEventArgs(ReadOnlyChunk chunk)
        {
            Chunk = chunk;
        }
    }
}