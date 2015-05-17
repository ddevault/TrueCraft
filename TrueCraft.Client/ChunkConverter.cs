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

        public ChunkConverter(GraphicsDevice graphics, IBlockRepository blockRepository)
        {
            ChunkQueue = new ConcurrentQueue<ReadOnlyChunk>();
            ChunkWorker = new Thread(new ThreadStart(DoChunks));
            ChunkWorker.IsBackground = true;
            ChunkWorker.Priority = ThreadPriority.Lowest;
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
            ChunkWorker.Abort();
        }

        private void DoChunks()
        {
            while (true)
            {
                ReadOnlyChunk chunk;
                if (ChunkQueue.Any())
                {
                    while (!ChunkQueue.TryDequeue(out chunk)) { }
                    var mesh = ProcessChunk(chunk);
                    mesh.Item1.Data = chunk;
                    mesh.Item2.Data = chunk;
                    Consumer(mesh.Item1, mesh.Item2);
                }
                Thread.Yield();
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

        private Tuple<Mesh, Mesh> ProcessChunk(ReadOnlyChunk chunk)
        {
            var opaqueVerticies = new List<VertexPositionNormalTexture>();
            var opaqueIndicies = new List<int>();

            var transparentVerticies = new List<VertexPositionNormalTexture>();
            var transparentIndicies = new List<int>();

            var drawableCoordinates = new HashSet<Coordinates3D>();

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
                            drawableCoordinates.Add(coords);
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
                                    drawableCoordinates.Add(next);
                            }
                        }
                    }
                }
            }
            foreach (var coords in drawableCoordinates)
            {
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
                        opaqueVerticies.Count, out i);
                    opaqueVerticies.AddRange(v);
                    opaqueIndicies.AddRange(i);
                }
                else
                {
                    int[] i;
                    var v = BlockRenderer.RenderBlock(provider, descriptor,
                        new Vector3(chunk.X * Chunk.Width + coords.X, coords.Y, chunk.Z * Chunk.Depth + coords.Z),
                        transparentVerticies.Count, out i);
                    transparentVerticies.AddRange(v);
                    transparentIndicies.AddRange(i);
                }
            }
            var meshes = new Tuple<Mesh, Mesh>(
                new Mesh(Graphics, opaqueVerticies.ToArray(), opaqueIndicies.ToArray(), false),
                new Mesh(Graphics, transparentVerticies.ToArray(), transparentIndicies.ToArray(), false));
            meshes.Item1.BoundingBox = boundingBox;
            meshes.Item2.BoundingBox = boundingBox;
            return meshes;
        }
    }
}