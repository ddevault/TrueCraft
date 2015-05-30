using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Linq;
using TrueCraft.Client.Rendering;
using TrueCraft.Core.World;
using TrueCraft.API;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using TrueCraft.API.Logic;
using System.Threading.Tasks;

namespace TrueCraft.Client.Rendering
{
    /// <summary>
    /// A daemon of sorts that creates meshes from chunks.
    /// Passing meshes back is NOT thread-safe.
    /// </summary>
    public class ChunkConverter
    {
        public class ChunkSorter : Comparer<Mesh>
        {
            public Coordinates3D Camera { get; set; }

            public ChunkSorter(Coordinates3D camera)
            {
                Camera = camera;
            }

            public override int Compare(Mesh _x, Mesh _y)
            {
                var x = ((ChunkMesh)_x).Chunk;
                var y = ((ChunkMesh)_y).Chunk;
                return (int)(new Coordinates3D(y.X * Chunk.Width, 0, y.Z * Chunk.Depth).DistanceTo(Camera) -
                    new Coordinates3D(x.X * Chunk.Width, 0, x.Z * Chunk.Depth).DistanceTo(Camera));
            }
        }

        public class MeshGeneratedEventArgs : EventArgs
        {
            public bool Transparent { get; set; }
            public ChunkMesh Mesh { get; set; }

            public MeshGeneratedEventArgs(ChunkMesh mesh, bool transparent)
            {
                Transparent = transparent;
                Mesh = mesh;
            }
        }

        public event EventHandler<MeshGeneratedEventArgs> MeshGenerated;

        private ConcurrentQueue<ReadOnlyChunk> ChunkQueue { get; set; }
        private GraphicsDevice GraphicsDevice { get; set; }
        private IBlockRepository BlockRepository { get; set; }
        private List<Tuple<Thread, CancellationTokenSource>> RenderThreads { get; set; }

        public ChunkConverter(GraphicsDevice graphics, IBlockRepository blockRepository)
        {
            ChunkQueue = new ConcurrentQueue<ReadOnlyChunk>();
            RenderThreads = new List<Tuple<Thread, CancellationTokenSource>>();
            var threads = Environment.ProcessorCount - 2;
            if (threads < 1)
                threads = 1;
            for (int i = 0; i < threads; i++)
            {
                var token = new CancellationTokenSource();
                var worker = new Thread(() => DoChunks(token));
                worker.IsBackground = true;
                RenderThreads.Add(new Tuple<Thread, CancellationTokenSource>(worker, token));
            }

            BlockRepository = blockRepository;
            GraphicsDevice = graphics;
        }

        public void QueueChunk(ReadOnlyChunk chunk)
        {
            ChunkQueue.Enqueue(chunk);
        }

        public void QueueHighPriorityChunk(ReadOnlyChunk chunk)
        {
            // TODO: Overwrite existing chunks
            Task.Factory.StartNew(() =>
            {
                ProcessChunk(chunk, new RenderState());
            });
        }

        public void Start()
        {
            RenderThreads.ForEach(t => t.Item1.Start());
        }

        public void Stop()
        {
            RenderThreads.ForEach(t => t.Item2.Cancel());
        }

        private void DoChunks(CancellationTokenSource token)
        {
            var state = new RenderState();
            while (true)
            {
                if (token.Token.IsCancellationRequested)
                    return;

                ReadOnlyChunk chunk;
                if (ChunkQueue.TryDequeue(out chunk))
                {
                    ProcessChunk(chunk, state);
                }
                Thread.Sleep(1);
            }
        }

        private static readonly Coordinates3D[] AdjacentCoordinates =
        {
            Coordinates3D.Up,
            Coordinates3D.Down,
            Coordinates3D.North,
            Coordinates3D.South,
            Coordinates3D.East,
            Coordinates3D.West
        };

        private class RenderState
        {
            public readonly List<VertexPositionNormalTexture> OpaqueVerticies = new List<VertexPositionNormalTexture>();
            public readonly List<int> OpaqueIndicies = new List<int>();
            public readonly List<VertexPositionNormalTexture> TransparentVerticies = new List<VertexPositionNormalTexture>();
            public readonly List<int> TransparentIndicies = new List<int>();
            public readonly HashSet<Coordinates3D> DrawableCoordinates = new HashSet<Coordinates3D>();
        }

        private void ProcessChunk(ReadOnlyChunk chunk, RenderState state)
        {
            state.OpaqueVerticies.Clear();
            state.OpaqueIndicies.Clear();
            state.TransparentVerticies.Clear();
            state.TransparentIndicies.Clear();
            state.DrawableCoordinates.Clear();

            for (byte x = 0; x < Chunk.Width; x++)
            {
                for (byte z = 0; z < Chunk.Depth; z++)
                {
                    for (byte y = 0; y < Chunk.Height; y++)
                    {
                        var coords = new Coordinates3D(x, y, z);
                        var id = chunk.GetBlockId(coords);
                        var provider = BlockRepository.GetBlockProvider(id);
                        if (id != 0 && (coords.X == 0 || coords.X == Chunk.Width - 1
                            || coords.Y == 0 || coords.Y == Chunk.Height - 1
                            || coords.Z == 0 || coords.Z == Chunk.Depth - 1))
                        {
                            state.DrawableCoordinates.Add(coords);
                        }
                        if (!provider.Opaque)
                        {
                            // Add adjacent blocks
                            foreach (var a in AdjacentCoordinates)
                            {
                                var next = coords + a;
                                if (next.X < 0 || next.X >= Chunk.Width
                                    || next.Y < 0 || next.Y >= Chunk.Height
                                    || next.Z < 0 || next.Z >= Chunk.Depth)
                                {
                                    continue;
                                }
                                if (chunk.GetBlockId(next) != 0)
                                    state.DrawableCoordinates.Add(next);
                            }
                        }
                    }
                }
            }
            var enumerator = state.DrawableCoordinates.GetEnumerator();
            for (int j = 0; j < state.DrawableCoordinates.Count; j++)
            {
                var coords = enumerator.Current;
                enumerator.MoveNext();
                var descriptor = new BlockDescriptor
                {
                    ID = chunk.GetBlockId(coords),
                    Metadata = chunk.GetMetadata(coords),
                    BlockLight = chunk.GetBlockLight(coords),
                    SkyLight = chunk.GetSkyLight(coords),
                    Coordinates = coords
                };
                var provider = BlockRepository.GetBlockProvider(descriptor.ID);
                if (provider.RenderOpaque)
                {
                    int[] i;
                    var v = BlockRenderer.RenderBlock(provider, descriptor,
                        new Vector3(chunk.X * Chunk.Width + coords.X, coords.Y, chunk.Z * Chunk.Depth + coords.Z),
                        state.OpaqueVerticies.Count, out i);
                    state.OpaqueVerticies.AddRange(v);
                    state.OpaqueIndicies.AddRange(i);
                }
                else
                {
                    int[] i;
                    var v = BlockRenderer.RenderBlock(provider, descriptor,
                        new Vector3(chunk.X * Chunk.Width + coords.X, coords.Y, chunk.Z * Chunk.Depth + coords.Z),
                        state.TransparentVerticies.Count, out i);
                    state.TransparentVerticies.AddRange(v);
                    state.TransparentIndicies.AddRange(i);
                }
            }
            var opaque = new ChunkMesh(chunk, GraphicsDevice, state.OpaqueVerticies.ToArray(), state.OpaqueIndicies.ToArray());
            var transparent = new ChunkMesh(chunk, GraphicsDevice, state.TransparentVerticies.ToArray(), state.TransparentIndicies.ToArray());
            if (MeshGenerated != null)
            {
                MeshGenerated(this, new MeshGeneratedEventArgs(opaque, false));
                MeshGenerated(this, new MeshGeneratedEventArgs(transparent, true));
            }
        }
    }
}
