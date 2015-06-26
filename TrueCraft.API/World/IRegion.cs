using System;
using System.Collections.Generic;

namespace TrueCraft.API.World
{
    public interface IRegion : IDisposable
    {
        IDictionary<Coordinates2D, IChunk> Chunks { get; }
        Coordinates2D Position { get; }

        IChunk GetChunk(Coordinates2D position, bool generate = true);
        void UnloadChunk(Coordinates2D position);
        void Save(string path);
    }
}