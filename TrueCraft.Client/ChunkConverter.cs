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

namespace TrueCraft.Client
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
                var x = (ReadOnlyChunk)_x.Data;
                var y = (ReadOnlyChunk)_y.Data;
                return (int)(new Coordinates3D(y.X * Chunk.Width, 0, y.Z * Chunk.Depth).DistanceTo(Camera) -
                    new Coordinates3D(x.X * Chunk.Width, 0, x.Z * Chunk.Depth).DistanceTo(Camera));
            }
        }

        public delegate void ChunkConsumer(Mesh opaqueMesh, Mesh transparentMesh);

        private ConcurrentQueue<ReadOnlyChunk> ChunkQueue { get; set; }
        private Thread ChunkWorker { get; set; }
        private GraphicsDevice Graphics { get; set; }
        private IBlockRepository BlockRepository { get; set; }
        private ChunkConsumer Consumer { get; set; }

        private CancellationTokenSource ChunksCts { get; set; }

        public ChunkConverter(GraphicsDevice graphics, IBlockRepository blockRepository)
        {
            ChunkQueue = new ConcurrentQueue<ReadOnlyChunk>();
            ChunkWorker = new Thread(DoChunks);
            ChunkWorker.IsBackground = true;
            ChunksCts = new CancellationTokenSource();
            BlockRepository = blockRepository;
            Graphics = graphics;
        }

        public void QueueChunk(ReadOnlyChunk chunk)
        {
            ChunkQueue.Enqueue(chunk);
        }

        public void Start(ChunkConsumer consumer)
        {
            Consumer = consumer;
            ChunkWorker.Start();
        }

        public void Stop()
        {
            ChunksCts.Cancel();
        }

        private void DoChunks()
        {
            while (true)
            {
                if (ChunksCts.Token.IsCancellationRequested)
                    return;

                ReadOnlyChunk chunk;
                if (ChunkQueue.TryDequeue(out chunk))
                {
                    var mesh = ProcessChunk(chunk);
                    mesh.Item1.Data = chunk;
                    mesh.Item2.Data = chunk;
                    Consumer(mesh.Item1, mesh.Item2);
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

        private Tuple<Mesh, Mesh> ProcessChunk(ReadOnlyChunk chunk)
        {
            OpaqueVerticies.Clear();
            OpaqueIndicies.Clear();
            TransparentVerticies.Clear();
            TransparentIndicies.Clear();
            DrawableCoordinates.Clear();

            var boundingBox = new Microsoft.Xna.Framework.BoundingBox(
                new Vector3(chunk.X * Chunk.Width, 0, chunk.Z * Chunk.Depth),
                new Vector3(chunk.X * Chunk.Width + Chunk.Width, Chunk.Height, chunk.Z * Chunk.Depth + Chunk.Depth));

            for (byte x = 0; x < Chunk.Width; x++)
            {
                for (byte z = 0; z < Chunk.Depth; z++)
                {
                    for (byte y = 0; y < Chunk.Height; y++)
                    {
                        var coords = new Coordinates3D(x, y, z);
                        var id = chunk.GetBlockId(coords);
                        var provider = BlockRepository.GetBlockProvider(id);
                        if (id != 0)
                            DrawableCoordinates.Add(coords);
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
            var meshes = new Tuple<Mesh, Mesh>(
                new Mesh(Graphics, OpaqueVerticies.ToArray(), OpaqueIndicies.ToArray(), false),
                new Mesh(Graphics, TransparentVerticies.ToArray(), TransparentIndicies.ToArray(), false));
            meshes.Item1.BoundingBox = boundingBox;
            meshes.Item2.BoundingBox = boundingBox;
            return meshes;
        }
    }
}
