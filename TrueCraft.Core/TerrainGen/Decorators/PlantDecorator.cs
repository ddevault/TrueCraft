using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.World;
using TrueCraft.Core.TerrainGen.Noise;
using TrueCraft.Core.World;
using TrueCraft.API;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.TerrainGen.Decorators
{
    public class PlantDecorator : IChunkDecorator
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
                    Coordinates3D Below = new Coordinates3D(X, Height, Z) + Coordinates3D.Down;
                    if (Noise.Value2D(BlockX, BlockZ) > 0.25)
                    {
                        Coordinates3D BlockLocation = new Coordinates3D(X, Height, Z);
                        Coordinates3D PlantPosition = BlockLocation + Coordinates3D.Up;
                        if (chunk.GetBlockID(BlockLocation).Equals(GrassBlock.BlockID) && PlantPosition.Y < Chunk.Height)
                        {
                            var Chance = ChanceNoise.Value2D(BlockX, BlockZ);
                            if (Chance < 1.3)
                            {
                                var BushNoise = ChanceNoise.Value2D(BlockX * 0.7, BlockZ * 0.7);
                                var GrassNoise = ChanceNoise.Value2D(BlockX * 0.3, BlockZ * 0.3);
                                if (Biome.Plants.Contains(PlantSpecies.Deadbush) && BushNoise > 1.75 && chunk.GetBlockID(BlockLocation).Equals(SandBlock.BlockID))
                                {
                                    GenerateDeadBush(chunk, PlantPosition);
                                    continue;
                                }
                                
                                if (Biome.Plants.Contains(PlantSpecies.TallGrass) && GrassNoise > 0.3 && GrassNoise < 0.95)
                                {
                                    byte Meta = (GrassNoise > 0.3 && GrassNoise < 0.45 && Biome.Plants.Contains(PlantSpecies.Fern)) ? (byte)0x2 : (byte)0x1;
                                    GenerateTallGrass(chunk, PlantPosition, Meta);
                                    continue;
                                }
                            }
                            else
                            {
                                var FlowerTypeNoise = ChanceNoise.Value2D(BlockX * 1.2, BlockZ * 1.2);
                                if (Biome.Plants.Contains(PlantSpecies.Rose) && FlowerTypeNoise > 0.8 && FlowerTypeNoise < 1.5)
                                {
                                    GenerateRose(chunk, PlantPosition);
                                }
                                else if (Biome.Plants.Contains(PlantSpecies.Dandelion) && FlowerTypeNoise <= 0.8)
                                {
                                    GenerateDandelion(chunk, PlantPosition);
                                }
                            }
                        }
                    }
                }
            }
        }

        void GenerateRose(IChunk chunk, Coordinates3D Location)
        {
            chunk.SetBlockID(Location, RoseBlock.BlockID);
        }

        void GenerateDandelion(IChunk chunk, Coordinates3D Location)
        {
            chunk.SetBlockID(Location, FlowerBlock.BlockID);
        }

        void GenerateTallGrass(IChunk chunk, Coordinates3D Location, byte Meta)
        {
            chunk.SetBlockID(Location, TallGrassBlock.BlockID);
            chunk.SetMetadata(Location, Meta);
        }

        void GenerateDeadBush(IChunk chunk, Coordinates3D Location)
        {
            chunk.SetBlockID(Location, DeadBushBlock.BlockID);
        }
    }
}