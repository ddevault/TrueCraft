using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.World;
using TrueCraft.API;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.Core.TerrainGen.Noise;
using TrueCraft.Core.World;
using TrueCraft.Core.TerrainGen.Decorations;

namespace TrueCraft.Core.TerrainGen.Decorators
{
    public class TreeDecorator : IChunkDecorator
    {
        public Perlin Noise { get; set; }
        public ClampNoise ChanceNoise { get; set; }

        public void Decorate(IWorld world, IChunk chunk, IBiomeRepository biomes)
        {
            Noise = new Perlin(world.Seed);
            ChanceNoise = new ClampNoise(Noise);
            ChanceNoise.MaxValue = 2;
            Coordinates2D? lastTree = null;
            for (int x = 0; x < 16; x++)
            {
                for (int z = 0; z < 16; z++)
                {
                    var biome = biomes.GetBiome(chunk.Biomes[x * Chunk.Width + z]);
                    var blockX = MathHelper.ChunkToBlockX(x, chunk.Coordinates.X);
                    var blockZ = MathHelper.ChunkToBlockZ(z, chunk.Coordinates.Z);
                    var height = chunk.HeightMap[x * Chunk.Width + z];

                    if (lastTree != null && lastTree.Value.DistanceTo(new Coordinates2D(x, z)) < biome.TreeDensity)
                        continue;

                    if (Noise.Value2D(blockX, blockZ) > 0.3)
                    {
                        var location = new Coordinates3D(x, height, z);
                        var id = chunk.GetBlockID(location);
                        var provider = world.BlockRepository.GetBlockProvider(id);
                        if (id == DirtBlock.BlockID || id == GrassBlock.BlockID || id == SnowfallBlock.BlockID
                            || (id != StationaryWaterBlock.BlockID && id != WaterBlock.BlockID
                                && id != LavaBlock.BlockID && id != StationaryLavaBlock.BlockID
                                && provider.BoundingBox == null))
                        {
                            if (provider.BoundingBox == null)
                                location.Y--;
                            var oakNoise = ChanceNoise.Value2D(blockX * 0.6, blockZ * 0.6);
                            var birchNoise = ChanceNoise.Value2D(blockX * 0.2, blockZ * 0.2);
                            var spruceNoise = ChanceNoise.Value2D(blockX * 0.35, blockZ * 0.35);

                            var baseCoordinates = location + Coordinates3D.Up;
                            if (biome.Trees.Contains(TreeSpecies.Oak) && oakNoise > 1.01 && oakNoise < 1.25)
                            {
                                var oak = new OakTree().GenerateAt(world, chunk, baseCoordinates);
                                if (oak)
                                {
                                    lastTree = new Coordinates2D(x, z);
                                    continue;
                                }
                            }
                            if (biome.Trees.Contains(TreeSpecies.Birch) && birchNoise > 0.3 && birchNoise < 0.95)
                            {
                                var birch = new BirchTree().GenerateAt(world, chunk, baseCoordinates);
                                if (birch)
                                {
                                    lastTree = new Coordinates2D(x, z);
                                    continue;
                                }
                            }
                            if (biome.Trees.Contains(TreeSpecies.Spruce) && spruceNoise < 0.75)
                            {
                                var random = new Random(world.Seed);
                                var type = random.Next(1, 2);
                                var generated = false;
                                if (type.Equals(1))
                                    generated = new PineTree().GenerateAt(world, chunk, baseCoordinates);
                                else
                                    generated = new ConiferTree().GenerateAt(world, chunk, baseCoordinates);

                                if (generated)
                                {
                                    lastTree = new Coordinates2D(x, z);
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}