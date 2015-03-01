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
            Perlin Noise = new Perlin();
            Noise.Seed = world.Seed;
            ClampNoise ChanceNoise = new ClampNoise(Noise);
            ChanceNoise.MaxValue = 1;
            for (int X = 0; X < 16; X++)
            {
                for (int Z = 0; Z < 16; Z++)
                {
                    IBiomeProvider Biome = biomes.GetBiome(chunk.Biomes[X * Chunk.Width + Z]);
                    var Height = chunk.HeightMap[X * Chunk.Width + Z];
                    var BlockX = MathHelper.ChunkToBlockX(X, chunk.Coordinates.X);
                    var BlockZ = MathHelper.ChunkToBlockZ(Z, chunk.Coordinates.Z);
                    if (Biome.Plants.Contains(PlantSpecies.SugarCane))
                    {
                        if (Noise.Value2D(BlockX, BlockZ) > 0.65)
                        {
                            Coordinates3D BlockLocation = new Coordinates3D(X, Height, Z);
                            Coordinates3D SugarCaneLocation = BlockLocation + Coordinates3D.Up;
                            var NeighboursWater = Decoration.NeighboursBlock(chunk, BlockLocation, WaterBlock.BlockID) || Decoration.NeighboursBlock(chunk, BlockLocation, StationaryWaterBlock.BlockID);
                            if (chunk.GetBlockID(BlockLocation).Equals(GrassBlock.BlockID) && NeighboursWater || chunk.GetBlockID(BlockLocation).Equals(SandBlock.BlockID) && NeighboursWater)
                            {
                                Random R = new Random(world.Seed);
                                double HeightChance = R.NextDouble();
                                int CaneHeight = 3;
                                if (HeightChance < 0.05)
                                    CaneHeight = 4;
                                else if (HeightChance > 0.1 && Height < 0.25)
                                    CaneHeight = 2;
                                Decoration.GenerateColumn(chunk, SugarCaneLocation, CaneHeight, SugarcaneBlock.BlockID);
                            }
                        }
                    }
                }
            }
        }
    }
}