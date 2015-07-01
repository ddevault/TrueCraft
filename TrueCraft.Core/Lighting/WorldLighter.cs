using System;
using TrueCraft.API.World;
using TrueCraft.Core.World;
using TrueCraft.API.Logic;
using TrueCraft.API;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace TrueCraft.Core.Lighting
{
    public class WorldLighter
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

        public WorldLighter(IWorld world, IBlockRepository blockRepository)
        {
            BlockRepository = blockRepository;
            World = world;
            PendingOperations = new List<LightingOperation>();
            HeightMaps = new Dictionary<Coordinates2D, byte[,]>();
            world.ChunkGenerated += (sender, e) => GenerateHeightMap(e.Chunk);
            world.ChunkLoaded += (sender, e) => GenerateHeightMap(e.Chunk);
            world.BlockChanged += (sender, e) => UpdateHeightMap(e.Position);
            foreach (var chunk in world)
                GenerateHeightMap(chunk);
        }

        private void GenerateHeightMap(IChunk chunk)
        {
            var map = new byte[Chunk.Width, Chunk.Depth];
            for (int x = 0; x < Chunk.Width; x++)
            {
                for (int z = 0; z < Chunk.Depth; z++)
                {
                    for (byte y = Chunk.Height - 1; y > 0; y--)
                    {
                        var id = chunk.GetBlockID(new Coordinates3D(x, y - 1, z));
                        var provider = BlockRepository.GetBlockProvider(id);
                        if (provider.LightModifier != 0)
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
            var map = HeightMaps[chunk.Coordinates];
            int x = adjusted.X; int z = adjusted.Z;
            for (byte y = Chunk.Height - 1; y > 0; y--)
            {
                var id = chunk.GetBlockID(new Coordinates3D(x, y - 1, z));
                var provider = BlockRepository.GetBlockProvider(id);
                if (provider.LightModifier != 0)
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
                {
                    emissiveness = 15;
                }
                if (emissiveness >= current)
                    return;
            }
            EnqueueOperation(new BoundingBox(new Vector3(x, y, z), new Vector3(x, y, z) + 1), op.SkyLight, op.Initial);
        }

        private void LightVoxel(int x, int y, int z, LightingOperation op)
        {
            // https://github.com/SirCmpwn/TrueCraft/wiki/Lighting
            var coords = new Coordinates3D(x, y, z);
            IChunk chunk;
            var adjustedCoords = World.FindBlockPosition(coords, out chunk, generate: false);
            if (chunk == null || !chunk.TerrainPopulated)
                return;
            var data = World.GetBlockData(coords);
            var provider = BlockRepository.GetBlockProvider(data.ID);
            byte current = op.SkyLight ? data.SkyLight : data.BlockLight;
            byte newLight = 0;
            byte opacity = Math.Max(provider.LightModifier, (byte)1);
            byte emissiveness = provider.Luminance;
            if (op.SkyLight)
            {
                if (y >= chunk.GetHeight((byte)adjustedCoords.X, (byte)adjustedCoords.Z))
                    emissiveness = 15;
            }
            else
                emissiveness = provider.Luminance;
            if (opacity < 15 || emissiveness != 0)
            {
                byte max = 0;
                for (int i = 0; i < Neighbors.Length; i++)
                {
                    if (World.IsValidPosition(coords + Neighbors[i]))
                    {
                        IChunk c;
                        var adjusted = World.FindBlockPosition(coords + Neighbors[i], out c, false);
                        if (c != null)
                        {
                            int val;
                            if (op.SkyLight)
                                val = c.GetSkyLight(adjusted);
                            else
                                val = c.GetBlockLight(adjusted);
                            var p = BlockRepository.GetBlockProvider(c.GetBlockID(adjusted));
                            val -= Math.Max(0, p.LightModifier - 1);
                            max = (byte)Math.Max(max, val);
                        }
                    }
                }
                newLight = (byte)Math.Max(max - opacity, emissiveness);
                if (newLight < 0)
                    newLight = 0;
            }
            if (newLight != current)
            {
                if (op.SkyLight)
                    World.SetSkyLight(coords, newLight);
                else
                    World.SetBlockLight(coords, newLight);
                byte propegated = (byte)Math.Max(newLight - 1, 0);

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
                    if (op.Box.Min.DistanceTo(box.Min) <= 2 && op.Box.Max.DistanceTo(box.Max) <= 2
                        && op.Box.Volume - box.Volume <= 2)
                    {
                        // TODO: Merge
                        //op.Box = new BoundingBox(Vector3.Min(op.Box.Min, box.Min), Vector3.Max(op.Box.Max, box.Max));
                        //return;
                    }
                }
                PendingOperations.Add(new LightingOperation { SkyLight = skyLight, Box = box, Initial = initial });
            }
        }
    }
}