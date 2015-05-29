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
            public Mesh Mesh { get; set; }

            public MeshGeneratedEventArgs(Mesh mesh, bool transparent)
            {
                Transparent = transparent;
                Mesh = mesh;
            }
        }

        public event EventHandler<MeshGeneratedEventArgs> MeshGenerated;

        private ConcurrentQueue<ReadOnlyChunk> ChunkQueue { get; set; }
        private Thread ChunkWorker { get; set; }
        private GraphicsDevice Graphics { get; set; }
        private IBlockRepository BlockRepository { get; set; }
        private CancellationTokenSource ThreadCancellationToken { get; set; }

        public ChunkConverter(GraphicsDevice graphics, IBlockRepository blockRepository)
        {
            ChunkQueue = new ConcurrentQueue<ReadOnlyChunk>();
            ChunkWorker = new Thread(DoChunks);
            ChunkWorker.IsBackground = true;
            ThreadCancellationToken = new CancellationTokenSource();
            BlockRepository = blockRepository;
            Graphics = graphics;
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
            ThreadCancellationToken.Cancel();
        }

        private void DoChunks()
        {
            while (true)
            {
                if (ThreadCancellationToken.Token.IsCancellationRequested)
                    return;

                ReadOnlyChunk chunk;
                if (ChunkQueue.TryDequeue(out chunk))
                {
                    ProcessChunk(chunk);
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

        private readonly List<VertexPositionNormalTexture> OpaqueVerticies = new List<VertexPositionNormalTexture>();
        private readonly List<int> OpaqueIndicies = new List<int>();
        private readonly List<VertexPositionNormalTexture> TransparentVerticies = new List<VertexPositionNormalTexture>();
        private readonly List<int> TransparentIndicies = new List<int>();
        private readonly HashSet<Coordinates3D> DrawableCoordinates = new HashSet<Coordinates3D>();

        private void ProcessChunk(ReadOnlyChunk chunk)
        {
            OpaqueVerticies.Clear();
            OpaqueIndicies.Clear();
            TransparentVerticies.Clear();
            TransparentIndicies.Clear();
            DrawableCoordinates.Clear();

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
                            DrawableCoordinates.Add(coords);
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
                                    DrawableCoordinates.Add(next);
                            }
                        }
                    }
                }
            }
            var enumerator = DrawableCoordinates.GetEnumerator();
            for (int j = 0; j < DrawableCoordinates.Count; j++)
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
                                OpaqueVerticies.Count, out i);
                    OpaqueVerticies.AddRange(v);
                    OpaqueIndicies.AddRange(i);
                }
                else
                {
                    int[] i;
                    var v = BlockRenderer.RenderBlock(provider, descriptor,
                                new Vector3(chunk.X * Chunk.Width + coords.X, coords.Y, chunk.Z * Chunk.Depth + coords.Z),
                                TransparentVerticies.Count, out i);
                    TransparentVerticies.AddRange(v);
                    TransparentIndicies.AddRange(i);
                }
            }
            var opaque = new ChunkMesh(chunk, Graphics, OpaqueVerticies.ToArray(), OpaqueIndicies.ToArray());
            var transparent = new ChunkMesh(chunk, Graphics, TransparentVerticies.ToArray(), TransparentIndicies.ToArray());
            if (MeshGenerated != null)
            {
                MeshGenerated(this, new MeshGeneratedEventArgs(opaque, false));
                MeshGenerated(this, new MeshGeneratedEventArgs(transparent, true));
            }
        }
    }
}
