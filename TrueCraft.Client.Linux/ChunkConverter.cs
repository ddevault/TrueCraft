using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Linq;

namespace TrueCraft.Client.Linux
{
    /// <summary>
    /// A daemon of sorts that creates meshes from chunks.
    /// Passing meshes back is NOT thread-safe.
    /// </summary>
    public class ChunkConverter
    {
        private ConcurrentQueue<ReadOnlyChunk> ChunkQueue { get; set; }
        private Thread ChunkWorker { get; set; }

        public ChunkConverter()
        {
            ChunkQueue = new ConcurrentQueue<ReadOnlyChunk>();
            ChunkWorker = new Thread(new ThreadStart(DoChunks));
        }

        public void QueueChunk(ReadOnlyChunk chunk)
        {
            ChunkQueue.Enqueue(chunk);
        }

        public void Start()
        {
            ChunkWorker.Start();
        }

        public void Stop()
        {
            ChunkWorker.Abort();
        }

        private void DoChunks()
        {
            bool idle = true;
            while (true)
            {
                ReadOnlyChunk chunk;
                if (ChunkQueue.Any())
                {
                    while (!ChunkQueue.TryDequeue(out chunk)) { }
                    // TODO: Create verticies from chunk
                    Console.WriteLine("Chunk worker received chunk at {0}, {1}", chunk.X, chunk.Z);
                }
                if (idle)
                    Thread.Sleep(100);
            }
        }
    }
}