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

        public int PendingChunks
        {
            get
            {
                return _items.Count + _priorityItems.Count;
            }
        }

        private ReadOnlyWorld World { get; set; }
        private TrueCraftGame Game { get; set; }
        private IBlockRepository BlockRepository { get; set; }

        public ChunkRenderer(ReadOnlyWorld world, TrueCraftGame game, IBlockRepository blockRepository)
            : base()
        {
            World = world;
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

        private static readonly VisibleFaces[] AdjacentCoordFaces =
        {
            VisibleFaces.Bottom,
            VisibleFaces.Top,
            VisibleFaces.South,
            VisibleFaces.North,
            VisibleFaces.West,
            VisibleFaces.East
        };

        protected override bool TryRender(ReadOnlyChunk item, out Mesh result)
        {
            var state = new RenderState();
            ProcessChunk(World, item, state);

            result = new ChunkMesh(item, Game, state.Verticies.ToArray(),
                state.OpaqueIndicies.ToArray(), state.TransparentIndicies.ToArray());

            return (result != null);
        }

        private class RenderState
        {
            public readonly List<VertexPositionNormalColorTexture> Verticies 
                = new List<VertexPositionNormalColorTexture>();
            public readonly List<int> OpaqueIndicies = new List<int>();
            public readonly List<int> TransparentIndicies = new List<int>();
            public readonly Dictionary<Coordinates3D, VisibleFaces> DrawableCoordinates
                = new Dictionary<Coordinates3D, VisibleFaces>();
        }

        private void AddBottomBlock(Coordinates3D coords, RenderState state, ReadOnlyChunk chunk)
        {
            var desiredFaces = VisibleFaces.None;
            if (coords.X == 0)
                desiredFaces |= VisibleFaces.West;
            else if (coords.X == Chunk.Width - 1)
                desiredFaces |= VisibleFaces.East;
            if (coords.Z == 0)
                desiredFaces |= VisibleFaces.North;
            else if (coords.Z == Chunk.Depth - 1)
                desiredFaces |= VisibleFaces.South;
            if (coords.Y == 0)
                desiredFaces |= VisibleFaces.Bottom;
            else if (coords.Y == Chunk.Depth - 1)
                desiredFaces |= VisibleFaces.Top;

            VisibleFaces faces;
            state.DrawableCoordinates.TryGetValue(coords, out faces);
            faces |= desiredFaces;
            state.DrawableCoordinates[coords] = desiredFaces;
        }

        private void AddAdjacentBlocks(Coordinates3D coords, RenderState state, ReadOnlyChunk chunk)
        {
            // Add adjacent blocks
            for (int i = 0; i < AdjacentCoordinates.Length; i++)
            {
                var next = coords + AdjacentCoordinates[i];
                if (next.X < 0 || next.X >= Chunk.Width
                    || next.Y < 0 || next.Y >= Chunk.Height
                    || next.Z < 0 || next.Z >= Chunk.Depth)
                {
                    continue;
                }
                var provider = BlockRepository.GetBlockProvider(chunk.GetBlockId(next));
                if (provider.Opaque)
                {
                    VisibleFaces faces;
                    if (!state.DrawableCoordinates.TryGetValue(next, out faces))
                        faces = VisibleFaces.None;
                    faces |= AdjacentCoordFaces[i];
                    state.DrawableCoordinates[next] = faces;
                }
            }
        }

        private void AddTransparentBlock(Coordinates3D coords, RenderState state, ReadOnlyChunk chunk)
        {
            // Add adjacent blocks
            VisibleFaces faces = VisibleFaces.None;
            for (int i = 0; i < AdjacentCoordinates.Length; i++)
            {
                var next = coords + AdjacentCoordinates[i];
                if (next.X < 0 || next.X >= Chunk.Width
                    || next.Y < 0 || next.Y >= Chunk.Height
                    || next.Z < 0 || next.Z >= Chunk.Depth)
                {
                    faces |= AdjacentCoordFaces[i];
                    continue;
                }
                if (chunk.GetBlockId(next) == 0)
                    faces |= AdjacentCoordFaces[i];
            }
            if (faces != VisibleFaces.None)
                state.DrawableCoordinates[coords] = faces;
        }

        private void UpdateFacesFromAdjacent(Coordinates3D adjacent, ReadOnlyChunk chunk,
            VisibleFaces mod, ref VisibleFaces faces)
        {
            if (chunk == null)
                return;
            var provider = BlockRepository.GetBlockProvider(chunk.GetBlockId(adjacent));
            if (!provider.Opaque)
                faces |= mod;
        }

        private void AddChunkBoundaryBlocks(Coordinates3D coords, RenderState state, ReadOnlyChunk chunk)
        {
            VisibleFaces faces;
            if (!state.DrawableCoordinates.TryGetValue(coords, out faces))
                faces = VisibleFaces.None;
            VisibleFaces oldFaces = faces;

            if (coords.X == 0)
            {
                var adjacent = coords;
                adjacent.X = Chunk.Width - 1;
                var nextChunk = World.GetChunk(chunk.Chunk.Coordinates + Coordinates2D.West);
                UpdateFacesFromAdjacent(adjacent, nextChunk, VisibleFaces.West, ref faces);
            }
            else if (coords.X == Chunk.Width - 1)
            {
                var adjacent = coords;
                adjacent.X = 0;
                var nextChunk = World.GetChunk(chunk.Chunk.Coordinates + Coordinates2D.East);
                UpdateFacesFromAdjacent(adjacent, nextChunk, VisibleFaces.East, ref faces);
            }

            if (coords.Z == 0)
            {
                var adjacent = coords;
                adjacent.Z = Chunk.Depth - 1;
                var nextChunk = World.GetChunk(chunk.Chunk.Coordinates + Coordinates2D.North);
                UpdateFacesFromAdjacent(adjacent, nextChunk, VisibleFaces.North, ref faces);
            }
            else if (coords.Z == Chunk.Depth - 1)
            {
                var adjacent = coords;
                adjacent.Z = 0;
                var nextChunk = World.GetChunk(chunk.Chunk.Coordinates + Coordinates2D.South);
                UpdateFacesFromAdjacent(adjacent, nextChunk, VisibleFaces.South, ref faces);
            }

            if (oldFaces != faces)
                state.DrawableCoordinates[coords] = faces;
        }

        private void ProcessChunk(ReadOnlyWorld world, ReadOnlyChunk chunk, RenderState state)
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
                        if (id != 0 && coords.Y == 0)
                            AddBottomBlock(coords, state, chunk);
                        if (!provider.Opaque)
                        {
                            AddAdjacentBlocks(coords, state, chunk);
                            if (id != 0)
                                AddTransparentBlock(coords, state, chunk);
                        }
                        else
                        {
                            if (coords.X == 0 || coords.X == Chunk.Width - 1 ||
                                coords.Z == 0 || coords.Z == Chunk.Depth - 1)
                            {
                                AddChunkBoundaryBlocks(coords, state, chunk);
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
                var c = coords.Key;
                var descriptor = new BlockDescriptor
                {
                    ID = chunk.GetBlockId(coords.Key),
                    Metadata = chunk.GetMetadata(coords.Key),
                    BlockLight = chunk.GetBlockLight(coords.Key),
                    SkyLight = chunk.GetSkyLight(coords.Key),
                    Coordinates = coords.Key,
                    Chunk = chunk.Chunk
                };
                var provider = BlockRepository.GetBlockProvider(descriptor.ID);
                if (provider.RenderOpaque)
                {
                    int[] i;
                    var v = BlockRenderer.RenderBlock(provider, descriptor, coords.Value,
                        new Vector3(chunk.X * Chunk.Width + c.X, c.Y, chunk.Z * Chunk.Depth + c.Z),
                        state.Verticies.Count, out i);
                    state.Verticies.AddRange(v);
                    state.OpaqueIndicies.AddRange(i);
                }
                else
                {
                    int[] i;
                    var v = BlockRenderer.RenderBlock(provider, descriptor, coords.Value,
                        new Vector3(chunk.X * Chunk.Width + c.X, c.Y, chunk.Z * Chunk.Depth + c.Z),
                        state.Verticies.Count, out i);
                    state.Verticies.AddRange(v);
                    state.TransparentIndicies.AddRange(i);
                }
            }
        }
    }

    [Flags]
    public enum VisibleFaces
    {
        None = 0,
        North = 1,
        South = 2,
        East = 4,
        West = 8,
        Top = 16,
        Bottom = 32,
        All = North | South | East | West | Top | Bottom
    }
}
