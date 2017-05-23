using System;
using System.Collections.Generic;

namespace TrueCraft.API.World
{
    public interface IRegion : IDisposable
    {
        IDictionary<Coordinates2D, IChunk> Chunks { get; }
        Coordinates2D Position { get; }

        IChunk GetChunk(Coordinates2D position, bool generate = true);
        /// <summary>
        /// Marks the chunk for saving in the next Save().
        /// </summary>
        void DamageChunk(Coordinates2D position);
        void UnloadChunk(Coordinates2D position);
        void Save(string path);
    }
}