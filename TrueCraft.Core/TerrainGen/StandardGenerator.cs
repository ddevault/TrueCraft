using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.World;
using TrueCraft.API;
using TrueCraft.Core.World;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.Core.TerrainGen.Noise;
using TrueCraft.Core.TerrainGen.Biomes;
using TrueCraft.Core.TerrainGen.Decorators;
using TrueCraft.Core.TerrainGen.Decorations;

namespace TrueCraft.Core.TerrainGen
{
    /// <summary>
    /// This terrain generator is still under heavy development. Use at your own risk.
    /// </summary>
    public class StandardGenerator : IChunkProvider
    {
        BiomeRepository Biomes = new BiomeRepository();
        Perlin HighNoise = new Perlin();
        Perlin LowNoise = new Perlin();
        Perlin BottomNoise = new Perlin();
        ClampNoise HighClamp;
        ClampNoise LowClamp;
        ClampNoise BottomClamp;
        ModifyNoise Modified;
        private int GroundLevel = 50;

        public StandardGenerator(bool SingleBiome = false, byte GenerateBiome = (byte)Biome.Plains)
        {
            this.SingleBiome = SingleBiome;
            this.GenerationBiome = GenerateBiome;
            HighNoise.Persistance = 1;
            HighNoise.Frequency = 0.013;
            HighNoise.Amplitude = 10;
            HighNoise.Octaves = 2;
            HighNoise.Lacunarity = 2;
            LowNoise.Persistance = 1;
            LowNoise.Frequency = 0.008;
            LowNoise.Amplitude = 14;
            LowNoise.Octaves = 2;
            LowNoise.Lacunarity = 2.5;
            BottomNoise.Persistance = 0.5;
            BottomNoise.Frequency = 0.013;
            BottomNoise.Amplitude = 5;
            BottomNoise.Octaves = 2;
            BottomNoise.Lacunarity = 1.5;
            HighClamp = new ClampNoise(HighNoise);
            HighClamp.MinValue = -10;
            HighClamp.MaxValue = 25;
            LowClamp = new ClampNoise(LowNoise);
            LowClamp.MinValue = -30;
            LowClamp.MaxValue = 30;
            BottomClamp = new ClampNoise(BottomNoise);
            BottomClamp.MinValue = -20;
            BottomClamp.MaxValue = 5;
            Modified = new ModifyNoise(HighClamp, LowClamp, NoiseModifier.Add);
            ChunkDecorators = new List<IChunkDecorator>();
            ChunkDecorators.Add(new WaterDecorator());
            ChunkDecorators.Add(new OreDecorator());
            ChunkDecorators.Add(new TreeDecorator());
            ChunkDecorators.Add(new FreezeDecorator());
            ChunkDecorators.Add(new PlantDecorator());
            ChunkDecorators.Add(new CactusDecorator());
            ChunkDecorators.Add(new SugarCaneDecorator());
            ChunkDecorators.Add(new DungeonDecorator(GroundLevel));
        }

        public IList<IChunkDecorator> ChunkDecorators { get; private set; }
        public Vector3 SpawnPoint { get; private set; }
        public bool SingleBiome { get; private set; }
        public byte GenerationBiome { get; private set; }

        public IChunk GenerateChunk(IWorld world, Coordinates2D coordinates)
        {
            const int featurePointDistance = 400;
            int seed = world.Seed;
            var worley = new CellNoise();
            worley.Seed = seed;
            HighNoise.Seed = seed;
            LowNoise.Seed = seed;
            var chunk = new Chunk(coordinates);
            for (int X = 0; X < 16; X++)
            {
                for (int Z = 0; Z < 16; Z++)
                {
                    var blockX = MathHelper.ChunkToBlockX(X, coordinates.X);
                    var blockZ = MathHelper.ChunkToBlockZ(Z, coordinates.Z);
                    const double lowClampRange = 5;
                    double lowClampMid = LowClamp.MaxValue - ((LowClamp.MaxValue + LowClamp.MinValue) / 2);
                    double lowClampValue = LowClamp.Value2D(blockX, blockZ);
                    if (lowClampValue > lowClampMid - lowClampRange && lowClampValue < lowClampMid + lowClampRange)
                    {
                        InvertNoise NewPrimary = new InvertNoise(HighClamp);
                        Modified.PrimaryNoise = NewPrimary;
                    }
                    else
                    {
                        //reset it after modifying the values
                        Modified = new ModifyNoise(HighClamp, LowClamp, NoiseModifier.Add);
                    }
                    Modified = new ModifyNoise(Modified, BottomClamp, NoiseModifier.Subtract);
                    var cellValue = worley.Value2D(blockX, blockZ);
                    var location = new Coordinates2D(blockX, blockZ);
                    if (world.BiomeDiagram.BiomeCells.Count < 1 || cellValue.Equals(1) && world.BiomeDiagram.ClosestCellPoint(location) >= featurePointDistance)
                    {
                        byte id = (SingleBiome) ? GenerationBiome : world.BiomeDiagram.GenerateBiome(seed, Biomes, location);
                        BiomeCell Cell = new BiomeCell(id, location);
                        world.BiomeDiagram.AddCell(Cell);
                    }

                    var biomeId = GetBiome(world, location);
                    IBiomeProvider Biome = Biomes.GetBiome(biomeId);
                    chunk.Biomes[X * Chunk.Width + Z] = biomeId;

                    var height = GetHeight(blockX, blockZ);
                    var surfaceHeight = height - Biome.SurfaceDepth;
                    chunk.HeightMap[X * Chunk.Width + Z] = height;
                    for (int Y = 0; Y <= height; Y++)
                    {
                        if (Y == 0)
                        {
                            chunk.SetBlockID(new Coordinates3D(X, Y, Z), BedrockBlock.BlockID);
                        }
                        else
                        {
                            if (Y.Equals(height) || Y < height && Y > surfaceHeight)
                            {
                                chunk.SetBlockID(new Coordinates3D(X, Y, Z), Biome.SurfaceBlock);
                            }
                            else
                            {
                                if (Y > surfaceHeight - Biome.FillerDepth)
                                {
                                    chunk.SetBlockID(new Coordinates3D(X, Y, Z), Biome.FillerBlock);
                                }
                                else
                                {
                                    chunk.SetBlockID(new Coordinates3D(X, Y, Z), StoneBlock.BlockID);
                                }
                            }
                        }
                    }
                }
            }
            foreach (IChunkDecorator ChunkDecorator in ChunkDecorators)
                ChunkDecorator.Decorate(world, chunk, Biomes);
            return chunk;
        }

        public Coordinates3D GetSpawn(IWorld world)
        {
            var chunk = GenerateChunk(world, Coordinates2D.Zero);
            var spawnPointHeight = chunk.HeightMap[0];
            return new Coordinates3D(0, spawnPointHeight + 1, 0);
        }

        byte GetBiome(IWorld world, Coordinates2D location)
        {
            if (SingleBiome)
                return GenerationBiome;
            return world.BiomeDiagram.GetBiome(location);
        }

        int GetHeight(int x, int z)
        {
            var NoiseValue = Modified.Value2D(x, z) + GroundLevel;
            if (NoiseValue < 0)
                NoiseValue = GroundLevel;
            if (NoiseValue > Chunk.Height)
                NoiseValue = Chunk.Height - 1;
            return (int)NoiseValue;
        }
    }
}