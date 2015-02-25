using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.World;
using TrueCraft.Core.World;
using TrueCraft.Core.TerrainGen.Noise;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.API;

namespace TrueCraft.Core.TerrainGen.Decorators
{
    public class CactusDecorator : IChunkDecorator
    {
        public void Decorate(IWorld world, IChunk chunk, IBiomeRepository biomes)
        {
            Perlin Noise = new Perlin();
            Noise.Seed = world.Seed;
            ClampNoise ChanceNoise = new ClampNoise(Noise);
            ChanceNoise.MaxValue = 2;
            for (int X = 0; X < 16; X++)
            {
                for (int Z = 0; Z < 16; Z++)
                {
                    IBiomeProvider Biome = biomes.GetBiome(chunk.Biomes[X * Chunk.Width + Z]);
                    var BlockX = MathHelper.ChunkToBlockX(X, chunk.Coordinates.X);
                    var BlockZ = MathHelper.ChunkToBlockZ(Z, chunk.Coordinates.Z);
                    var Height = chunk.HeightMap[X * Chunk.Width + Z];
                    if (Biome.Plants.Contains(PlantSpecies.Cactus) && ChanceNoise.Value2D(BlockX, BlockZ) > 1.7)
                    {
                        Coordinates3D BlockLocation = new Coordinates3D(X, Height, Z);
                        Coordinates3D CactiPosition = BlockLocation + Coordinates3D.Up;
                        if (chunk.GetBlockID(BlockLocation).Equals(SandBlock.BlockID))
                        {
                            var HeightChance = ChanceNoise.Value2D(BlockX, BlockZ);
                            var CactusHeight = (HeightChance < 1.4) ? 2 : 3;
                            for (int Y = CactiPosition.Y; Y < CactiPosition.Y + CactusHeight; Y++)
                            {
                                chunk.SetBlockID(new Coordinates3D(CactiPosition.X, Y, CactiPosition.Z), CactusBlock.BlockID);
                            }
                        }
                    }
                }
            }
        }
    }
}