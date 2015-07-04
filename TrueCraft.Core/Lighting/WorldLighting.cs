using System;
using TrueCraft.API.World;
using TrueCraft.Core.World;
using TrueCraft.API.Logic;
using TrueCraft.API;
using System.Collections.Generic;
using System.Diagnostics;

namespace TrueCraft.Core.Lighting
{
    // https://github.com/SirCmpwn/TrueCraft/wiki/Lighting

    // Note: Speed-critical code
    public class WorldLighting
    {
        private static readonly Coordinates3D[] Neighbors =
        {
            Coordinates3D.Up,
            Coordinates3D.Down,
            Coordinates3D.East,
            Coordinates3D.West,
            Coordinates3D.North,
            Coordinates3D.South
        };

        private struct LightingOperation
        {
            public BoundingBox Box { get; set; }
            public bool SkyLight { get; set; }
            public bool Initial { get; set; }
        }

        public IBlockRepository BlockRepository { get; set; }
        public IWorld World { get; set; }

        private object _Lock = new object();
        private List<LightingOperation> PendingOperations { get; set; }
        private Dictionary<Coordinates2D, byte[,]> HeightMaps { get; set; }

        public WorldLighting(IWorld world, IBlockRepository blockRepository)
        {
            BlockRepository = blockRepository;
            World = world;
            PendingOperations = new List<LightingOperation>();
            HeightMaps = new Dictionary<Coordinates2D, byte[,]>();
            world.ChunkGenerated += (sender, e) => GenerateHeightMap(e.Chunk);
            world.ChunkLoaded += (sender, e) => GenerateHeightMap(e.Chunk);
            world.BlockChanged += (sender, e) =>
            {
                if (e.NewBlock.ID != e.OldBlock.ID)
                    UpdateHeightMap(e.Position);
            };
            foreach (var chunk in world)
                GenerateHeightMap(chunk);
        }

        private void GenerateHeightMap(IChunk chunk)
        {
            Coordinates3D coords;
            var map = new byte[Chunk.Width, Chunk.Depth];
            for (byte x = 0; x < Chunk.Width; x++)
            {
                for (byte z = 0; z < Chunk.Depth; z++)
                {
                    for (byte y = (byte)(chunk.GetHeight(x, z) + 2); y > 0; y--)
                    {
                        if (y >= Chunk.Height)
                            continue;
                        coords.X = x; coords.Y = y - 1; coords.Z = z;
                        var id = chunk.GetBlockID(coords);
                        if (id == 0)
                            continue;
                        var provider = BlockRepository.GetBlockProvider(id);
                        if (provider.LightOpacity != 0)
                        {
                            map[x, z] = y;
                            break;
                        }
                    }
                }
            }
            HeightMaps[chunk.Coordinates] = map;
        }

        private void UpdateHeightMap(Coordinates3D coords)
        {
            IChunk chunk;
            var adjusted = World.FindBlockPosition(coords, out chunk, generate: false);
            if (!HeightMaps.ContainsKey(chunk.Coordinates))
                return;
            var map = HeightMaps[chunk.Coordinates];
            byte x = (byte)adjusted.X; byte z = (byte)adjusted.Z;
            Coordinates3D _;
            for (byte y = (byte)(chunk.GetHeight(x, z) + 2); y > 0; y--)
            {
                if (y >= Chunk.Height)
                    continue;
                _.X = x; _.Y = y - 1; _.Z = z;
                var id = chunk.GetBlockID(_);
                if (id == 0)
                    continue;
                var provider = BlockRepository.GetBlockProvider(id);
                if (provider.LightOpacity != 0)
                {
                    map[x, z] = y;
                    break;
                }
            }
        }

        private void LightBox(LightingOperation op)
        {
            var chunk = World.FindChunk((Coordinates3D)op.Box.Center, generate: false);
            if (chunk == null || !chunk.TerrainPopulated)
                return;
            for (int x = (int)op.Box.Min.X; x < (int)op.Box.Max.X; x++)
            for (int z = (int)op.Box.Min.Z; z < (int)op.Box.Max.Z; z++)
            for (int y = (int)op.Box.Max.Y - 1; y >= (int)op.Box.Min.Y; y--)
            {
                LightVoxel(x, y, z, op);
            }
        }

        /// <summary>
        /// Propegates a lighting change to an adjacent voxel (if neccesary)
        /// </summary>
        private void PropegateLightEvent(int x, int y, int z, byte value, LightingOperation op)
        {
            var coords = new Coordinates3D(x, y, z);
            if (!World.IsValidPosition(coords))
                return;
            IChunk chunk;
            var adjustedCoords = World.FindBlockPosition(coords, out chunk, generate: false);
            if (chunk == null || !chunk.TerrainPopulated)
                return;
            byte current = op.SkyLight ? World.GetSkyLight(coords) : World.GetBlockLight(coords);
            if (value == current)
                return;
            var provider = BlockRepository.GetBlockProvider(World.GetBlockID(coords));
            if (op.Initial)
            {
                byte emissiveness = provider.Luminance;
                if (chunk.GetHeight((byte)adjustedCoords.X, (byte)adjustedCoords.Z) <= y)
                    emissiveness = 15;
                if (emissiveness >= current)
                    return;
            }
            EnqueueOperation(new BoundingBox(new Vector3(x, y, z), new Vector3(x, y, z) + 1), op.SkyLight, op.Initial);
        }

        /// <summary>
        /// Computes the correct lighting value for a given voxel.
        /// </summary>
        private void LightVoxel(int x, int y, int z, LightingOperation op)
        {
            var coords = new Coordinates3D(x, y, z);

            IChunk chunk;
            var adjustedCoords = World.FindBlockPosition(coords, out chunk, generate: false);

            if (chunk == null || !chunk.TerrainPopulated) // Move on if this chunk is empty
                return;

            var id = World.GetBlockID(coords);
            var provider = BlockRepository.GetBlockProvider(id);

            // The opacity of the block determines the amount of light it receives from
            // neighboring blocks. This is subtracted from the max of the neighboring
            // block values. We must subtract at least 1.
            byte opacity = Math.Max(provider.LightOpacity, (byte)1);

            byte current = op.SkyLight ? World.GetSkyLight(coords) : World.GetBlockLight(coords);
            byte final = 0;

            // Calculate emissiveness
            byte emissiveness = provider.Luminance;
            if (op.SkyLight)
            {
                var height = HeightMaps[chunk.Coordinates][adjustedCoords.X, adjustedCoords.Z];
                // For skylight, the emissiveness is 15 if y >= height
                if (y >= height)
                    emissiveness = 15;
                else
                {
                    if (provider.LightOpacity >= 15)
                        emissiveness = 0;
                }
            }
            
            if (opacity < 15 || emissiveness != 0)
            {
                // Compute the light based on the max of the neighbors
                byte max = 0;
                for (int i = 0; i < Neighbors.Length; i++)
                {
                    if (World.IsValidPosition(coords + Neighbors[i]))
                    {
                        IChunk c;
                        var adjusted = World.FindBlockPosition(coords + Neighbors[i], out c, generate: false);
                        if (c != null) // We don't want to generate new chunks just to light this voxel
                        {
                            byte val;
                            if (op.SkyLight)
                                val = c.GetSkyLight(adjusted);
                            else
                                val = c.GetBlockLight(adjusted);
                            max = Math.Max(max, val);
                        }
                    }
                }
                // final = MAX(max - opacity, emissiveness, 0)
                final = (byte)Math.Max(max - opacity, emissiveness);
                if (final < 0)
                    final = 0;
            }

            if (final != current)
            {
                // Apply changes
                if (op.SkyLight)
                    chunk.SetSkyLight(adjustedCoords, final);
                else
                    chunk.SetBlockLight(adjustedCoords, final);
                
                byte propegated = (byte)Math.Max(final - 1, 0);

                // Propegate lighting change to neighboring blocks
                PropegateLightEvent(x - 1, y, z, propegated, op);
                PropegateLightEvent(x, y - 1, z, propegated, op);
                PropegateLightEvent(x, y, z - 1, propegated, op);
                if (x + 1 >= op.Box.Max.X)
                    PropegateLightEvent(x + 1, y, z, propegated, op);
                if (y + 1 >= op.Box.Max.Y)
                    PropegateLightEvent(x,  y + 1, z, propegated, op);
                if (z + 1 >= op.Box.Max.Z)
                    PropegateLightEvent(x, y, z + 1, propegated, op);
            }
        }

        public bool TryLightNext()
        {
            LightingOperation op;
            lock (_Lock)
            {
                if (PendingOperations.Count == 0)
                    return false;
                op = PendingOperations[0];
                PendingOperations.RemoveAt(0);
            }
            LightBox(op);
            return true;
        }

        public void EnqueueOperation(BoundingBox box, bool skyLight, bool initial = false)
        {
            lock (_Lock)
            {
                // Try to merge with existing operation
                for (int i = PendingOperations.Count - 1; i > PendingOperations.Count - 5 && i > 0; i--)
                {
                    var op = PendingOperations[i];
                    if (op.Box.Intersects(box))
                    {
                        op.Box = new BoundingBox(Vector3.Min(op.Box.Min, box.Min), Vector3.Max(op.Box.Max, box.Max));
                        return;
                    }
                }
                PendingOperations.Add(new LightingOperation { SkyLight = skyLight, Box = box, Initial = initial });
            }
        }

        private void SetUpperVoxels(IChunk chunk)
        {
            for (int x = 0; x < Chunk.Width; x++)
            for (int z = 0; z < Chunk.Depth; z++)
            for (int y = chunk.MaxHeight + 1; y < Chunk.Height; y++)
                chunk.SetSkyLight(new Coordinates3D(x, y, z), 15);
        }

        /// <summary>
        /// Queues the initial lighting pass for a newly generated chunk.
        /// </summary>
        public void InitialLighting(IChunk chunk, bool flush = true)
        {
            // Set voxels above max height to 0xFF
            SetUpperVoxels(chunk);
            var coords = chunk.Coordinates * new Coordinates2D(Chunk.Width, Chunk.Depth);
            EnqueueOperation(new BoundingBox(new Vector3(coords.X, 0, coords.Z),
                new Vector3(coords.X + Chunk.Width, chunk.MaxHeight + 2, coords.Z + Chunk.Depth)),
                true, true);
            while (flush && TryLightNext())
            {
            }
        }
    }
}