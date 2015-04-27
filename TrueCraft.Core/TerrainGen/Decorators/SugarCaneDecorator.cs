using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.World;
using TrueCraft.Core.World;
using TrueCraft.Core.TerrainGen.Noise;
using TrueCraft.API;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.Core.TerrainGen.Decorations;

namespace TrueCraft.Core.TerrainGen.Decorators
{
    public class SugarCaneDecorator : IChunkDecorator
    {
        public void Decorate(IWorld world, IChunk chunk, IBiomeRepository biomes)
        {
            var noise = new Perlin();
            noise.Seed = world.Seed;
            var chanceNoise = new ClampNoise(noise);
            chanceNoise.MaxValue = 1;
            for (int x = 0; x < 16; x++)
            {
                for (int z = 0; z < 16; z++)
                {
                    var biome = biomes.GetBiome(chunk.Biomes[x * Chunk.Width + z]);
                    var height = chunk.HeightMap[x * Chunk.Width + z];
                    var blockX = MathHelper.ChunkToBlockX(x, chunk.Coordinates.X);
                    var blockZ = MathHelper.ChunkToBlockZ(z, chunk.Coordinates.Z);
                    if (biome.Plants.Contains(PlantSpecies.SugarCane))
                    {
                        if (noise.Value2D(blockX, blockZ) > 0.65)
                        {
                            var blockLocation = new Coordinates3D(x, height, z);
                            var sugarCaneLocation = blockLocation + Coordinates3D.Up;
                            var neighborsWater = Decoration.NeighboursBlock(chunk, blockLocation, WaterBlock.BlockID) || Decoration.NeighboursBlock(chunk, blockLocation, StationaryWaterBlock.BlockID);
                            if (chunk.GetBlockID(blockLocation).Equals(GrassBlock.BlockID) && neighborsWater || chunk.GetBlockID(blockLocation).Equals(SandBlock.BlockID) && neighborsWater)
                            {
                                var random = new Random(world.Seed);
                                double heightChance = random.NextDouble();
                                int caneHeight = 3;
                                if (heightChance < 0.05)
                                    caneHeight = 4;
                                else if (heightChance > 0.1 && height < 0.25)
                                    caneHeight = 2;
                                Decoration.GenerateColumn(chunk, sugarCaneLocation, caneHeight, SugarcaneBlock.BlockID);
                            }
                        }
                    }
                }
            }
        }
    }
}