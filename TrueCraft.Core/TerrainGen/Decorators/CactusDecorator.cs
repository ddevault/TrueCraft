using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.World;
using TrueCraft.Core.World;
using TrueCraft.Core.TerrainGen.Noise;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.API;
using TrueCraft.API.Logic;
using TrueCraft.Core.TerrainGen.Decorations;

namespace TrueCraft.Core.TerrainGen.Decorators
{
    public class CactusDecorator : IChunkDecorator
    {
        public void Decorate(IWorldSeed world, ISpatialBlockInformationProvider chunk, IBiomeRepository biomes, IBlockRepository blockRepository)
        {
            var noise = new Perlin(world.Seed);
            var chanceNoise = new ClampNoise(noise);
            chanceNoise.MaxValue = 2;
            for (int x = 0; x < 16; x++)
            {
                for (int z = 0; z < 16; z++)
                {
                    var biome = biomes.GetBiome(chunk.Biomes[x * Chunk.Width + z]);
                    var blockX = MathHelper.ChunkToBlockX(x, chunk.Coordinates.X);
                    var blockZ = MathHelper.ChunkToBlockZ(z, chunk.Coordinates.Z);
                    var height = chunk.HeightMap[x * Chunk.Width + z];
                    if (biome.Plants.Contains(PlantSpecies.Cactus) && chanceNoise.Value2D(blockX, blockZ) > 1.7)
                    {
                        var blockLocation = new Coordinates3D(x, height, z);
                        var cactiPosition = blockLocation + Coordinates3D.Up;
                        if (chunk.GetBlockID(blockLocation).Equals(SandBlock.BlockID))
                        {
                            var HeightChance = chanceNoise.Value2D(blockX, blockZ);
                            var CactusHeight = (HeightChance < 1.4) ? 2 : 3;
                            Decoration.GenerateColumn(chunk, cactiPosition, CactusHeight, CactusBlock.BlockID);
                        }
                    }
                }
            }
        }
    }
}