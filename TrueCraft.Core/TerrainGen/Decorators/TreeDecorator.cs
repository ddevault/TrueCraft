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
        Perlin Noise;
        ClampNoise ChanceNoise;
        public void Decorate(IWorld world, IChunk chunk, IBiomeRepository biomes)
        {
            Noise = new Perlin();
            Noise.Seed = world.Seed;
            ChanceNoise = new ClampNoise(Noise);
            ChanceNoise.MaxValue = 2;
            Coordinates2D LastTree = new Coordinates2D();
            for (int X = 0; X < 16; X++)
            {
                for (int Z = 0; Z < 16; Z++)
                {
                    IBiomeProvider Biome = biomes.GetBiome(chunk.Biomes[X * Chunk.Width + Z]);
                    var BlockX = MathHelper.ChunkToBlockX(X, chunk.Coordinates.X);
                    var BlockZ = MathHelper.ChunkToBlockZ(Z, chunk.Coordinates.Z);
                    var Height = chunk.HeightMap[X * Chunk.Width + Z];

                    if (!LastTree.Equals(null) && LastTree.DistanceTo(new Coordinates2D(X, Z)) < Biome.TreeDensity)
                    {
                        continue;
                    }

                    if (Noise.Value2D(BlockX, BlockZ) > 0.3)
                    {
                        Coordinates3D location = new Coordinates3D(X, Height, Z);
                        if (chunk.GetBlockID(location) == GrassBlock.BlockID || chunk.GetBlockID(location) == SnowfallBlock.BlockID)
                        {
                            var Chance = ChanceNoise.Value2D(BlockX, BlockZ);
                            var OakNoise = ChanceNoise.Value2D(BlockX * 0.6, BlockZ * 0.6);
                            var BirchNoise = ChanceNoise.Value2D(BlockX * 0.2, BlockZ * 0.2);
                            var SpruceNoise = ChanceNoise.Value2D(BlockX * 0.35, BlockZ * 0.35);

                            Coordinates3D Base = location + Coordinates3D.Up;
                            if (Biome.Trees.Contains(TreeSpecies.Oak) && OakNoise > 1.01 && OakNoise < 1.25)
                            {
                                var Oak = new OakTree().GenerateAt(world, chunk, Base);
                                if (Oak)
                                {
                                    LastTree = new Coordinates2D(X, Z);
                                    continue;
                                }
                            }
                            if (Biome.Trees.Contains(TreeSpecies.Birch) && BirchNoise > 0.3 && BirchNoise < 0.95)
                            {
                                var Birch = new BirchTree().GenerateAt(world, chunk, Base);
                                if (Birch)
                                {
                                    LastTree = new Coordinates2D(X, Z);
                                    continue;
                                }
                            }
                            if (Biome.Trees.Contains(TreeSpecies.Spruce) && SpruceNoise < 0.75)
                            {
                                Random R = new Random(world.Seed);
                                var type = R.Next(1, 2);
                                var Generated = false;
                                if (type.Equals(1))
                                {
                                    Generated = new PineTree().GenerateAt(world, chunk, Base);
                                }
                                else
                                {
                                    Generated = new ConiferTree().GenerateAt(world, chunk, Base);
                                }

                                if (Generated)
                                {
                                    LastTree = new Coordinates2D(X, Z);
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