using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.World;
using TrueCraft.Core.World;
using TrueCraft.Core.TerrainGen.Noise;
using TrueCraft.API;
using TrueCraft.API.Logic;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.Core.TerrainGen.Decorations;

namespace TrueCraft.Core.TerrainGen.Decorators
{
    public class SugarCaneDecorator : IChunkDecorator
    {
        private readonly NoiseGen suppliedNoise;

        public SugarCaneDecorator()
        {
            suppliedNoise = null;
        }

        public SugarCaneDecorator(NoiseGen suppliedNoiseSource)
        {
            suppliedNoise = suppliedNoiseSource;
        }
        public void Decorate(IWorldSeed world, ISpatialBlockInformationProvider chunk, IBiomeRepository biomes, IBlockRepository blockRepository)
        {
            NoiseGen noise;
            if (suppliedNoise == null)
            {
                noise = new Perlin(world.Seed);
            }
            else
            {
                noise = suppliedNoise;
            }
            var random = new Random(world.Seed);

            var chanceNoise = new ClampNoise(noise);
            chanceNoise.MaxValue = 1;
            for (int x = 0; x < 16; x++)
            {
                for (int z = 0; z < 16; z++)
                {
                    var chunkBiome = chunk.Biomes[x * Chunk.Width + z];
                    var biome = biomes.GetBiome(chunkBiome);
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
                            var sugarCaneCanGrowOnCurrentBlock = (chunk.GetBlockID(blockLocation).Equals(GrassBlock.BlockID) || chunk.GetBlockID(blockLocation).Equals(SandBlock.BlockID));
                            if (neighborsWater && sugarCaneCanGrowOnCurrentBlock)
                            {
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