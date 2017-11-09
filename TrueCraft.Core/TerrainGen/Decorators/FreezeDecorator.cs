using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.World;
using TrueCraft.Core.World;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.API;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.TerrainGen.Decorators
{
    class FreezeDecorator : IChunkDecorator
    {
        public void Decorate(IWorldSeed world, IChunk chunk, IBiomeRepository biomes, IBlockRepository blockRepository)
        {
            for (int x = 0; x < 16; x++)
            {
                for (int z = 0; z < 16; z++)
                {
                    var biome = biomes.GetBiome(chunk.Biomes[x * Chunk.Width + z]);
                    if (biome.Temperature < 0.15)
                    {
                        var height = chunk.HeightMap[x * Chunk.Width + z];
                        for (int y = height; y < Chunk.Height; y++)
                        {
                            var location = new Coordinates3D(x, y, z);
                            if (chunk.GetBlockID(location).Equals(StationaryWaterBlock.BlockID) || chunk.GetBlockID(location).Equals(WaterBlock.BlockID))
                                chunk.SetBlockID(location, IceBlock.BlockID);
                            else
                            {
                                var below = chunk.GetBlockID(location);
                                byte[] whitelist =
                                {
                                    DirtBlock.BlockID,
                                    GrassBlock.BlockID,
                                    IceBlock.BlockID,
                                    LeavesBlock.BlockID
                                };
                                if (y == height && whitelist.Any(w => w == below))
                                {
                                    if (chunk.GetBlockID(location).Equals(IceBlock.BlockID) && CoverIce(chunk, biomes, location))
                                        chunk.SetBlockID((location + Coordinates3D.Up), SnowfallBlock.BlockID);
                                    else if (!chunk.GetBlockID(location).Equals(SnowfallBlock.BlockID) && !chunk.GetBlockID(location).Equals(AirBlock.BlockID))
                                        chunk.SetBlockID((location + Coordinates3D.Up), SnowfallBlock.BlockID);
                                }
                            }
                        }
                    }
                }
            }
        }

        bool CoverIce(IChunk chunk, IBiomeRepository biomes, Coordinates3D location)
        {
            const int maxDistance = 4;
            var adjacent = new[] {
                location + new Coordinates3D(-maxDistance, 0, 0),
                location + new Coordinates3D(maxDistance, 0, 0),
                location + new Coordinates3D(0, 0, maxDistance),
                location + new Coordinates3D(0, 0, -maxDistance),
            };
            for (int i = 0; i < adjacent.Length; i++)
            {
                var check = adjacent[i];
                if (check.X < 0 || check.X >= Chunk.Width || check.Z < 0 || check.Z >= Chunk.Depth || check.Y < 0 || check.Y >= Chunk.Height)
                    return false;
                var biome = biomes.GetBiome(chunk.Biomes[check.X * Chunk.Width + check.Z]);
                if (chunk.GetBlockID(check).Equals(biome.SurfaceBlock) || chunk.GetBlockID(check).Equals(biome.FillerBlock))
                    return true;
            }
            return false;
        }
    }
}