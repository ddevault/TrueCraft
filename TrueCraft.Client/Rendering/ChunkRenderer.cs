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
    public class ChunkRenderer : Renderer<ReadOnlyChunk>
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

        private TrueCraftGame Game { get; set; }
        private IBlockRepository BlockRepository { get; set; }

        public ChunkRenderer(TrueCraftGame game, IBlockRepository blockRepository)
            : base()
        {
            BlockRepository = blockRepository;
            Game = game;
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

        protected override bool TryRender(ReadOnlyChunk item, out Mesh result)
        {
            var state = new RenderState();
            ProcessChunk(item, state);

            result = new ChunkMesh(item, Game, state.Verticies.ToArray(),
                state.OpaqueIndicies.ToArray(), state.TransparentIndicies.ToArray());

            return (result != null);
            
        }

        private class RenderState
        {
            public readonly List<VertexPositionNormalColorTexture> Verticies = new List<VertexPositionNormalColorTexture>();
            public readonly List<int> OpaqueIndicies = new List<int>();
            public readonly List<int> TransparentIndicies = new List<int>();
            public readonly HashSet<Coordinates3D> DrawableCoordinates = new HashSet<Coordinates3D>();
        }

        private void ProcessChunk(ReadOnlyChunk chunk, RenderState state)
        {
            state.Verticies.Clear();
            state.OpaqueIndicies.Clear();
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
            for (int j = 0; j <= state.DrawableCoordinates.Count; j++)
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
                        state.Verticies.Count, out i);
                    state.Verticies.AddRange(v);
                    state.OpaqueIndicies.AddRange(i);
                }
                else
                {
                    int[] i;
                    var v = BlockRenderer.RenderBlock(provider, descriptor,
                        new Vector3(chunk.X * Chunk.Width + coords.X, coords.Y, chunk.Z * Chunk.Depth + coords.Z),
                        state.Verticies.Count, out i);
                    state.Verticies.AddRange(v);
                    state.TransparentIndicies.AddRange(i);
                }
            }
        }
    }
}
