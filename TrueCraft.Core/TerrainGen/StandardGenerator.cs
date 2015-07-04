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
        Perlin CaveNoise = new Perlin();
        ClampNoise HighClamp;
        ClampNoise LowClamp;
        ClampNoise BottomClamp;
        ModifyNoise FinalNoise;
        bool EnableCaves;
        private const int GroundLevel = 50;

        public StandardGenerator(TrueCraft.Core.World.World world) : this()
        {
            // TODO: Do we want to do anything with that world?
        }

        public StandardGenerator(bool singleBiome = false, bool enableCaves = true, byte generateBiome = (byte)Biome.Plains)
        {
            SingleBiome = singleBiome;
            GenerationBiome = generateBiome;
            EnableCaves = enableCaves;

            CaveNoise.Octaves = 3;
            CaveNoise.Amplitude = 0.05;
            CaveNoise.Persistance = 2;
            CaveNoise.Frequency = 0.05;
            CaveNoise.Lacunarity = 2;

            HighNoise.Persistance = 1;
            HighNoise.Frequency = 0.013;
            HighNoise.Amplitude = 10;
            HighNoise.Octaves = 2;
            HighNoise.Lacunarity = 2;

            LowNoise.Persistance = 1;
            LowNoise.Frequency = 0.004;
            LowNoise.Amplitude = 35;
            LowNoise.Octaves = 2;
            LowNoise.Lacunarity = 2.5;

            BottomNoise.Persistance = 0.5;
            BottomNoise.Frequency = 0.013;
            BottomNoise.Amplitude = 5;
            BottomNoise.Octaves = 2;
            BottomNoise.Lacunarity = 1.5;

            HighClamp = new ClampNoise(HighNoise);
            HighClamp.MinValue = -30;
            HighClamp.MaxValue = 50;

            LowClamp = new ClampNoise(LowNoise);
            LowClamp.MinValue = -30;
            LowClamp.MaxValue = 30;

            BottomClamp = new ClampNoise(BottomNoise);
            BottomClamp.MinValue = -20;
            BottomClamp.MaxValue = 5;

            FinalNoise = new ModifyNoise(HighClamp, LowClamp, NoiseModifier.Add);

            ChunkDecorators = new List<IChunkDecorator>();
            ChunkDecorators.Add(new LiquidDecorator());
            ChunkDecorators.Add(new OreDecorator());
            ChunkDecorators.Add(new PlantDecorator());
            ChunkDecorators.Add(new TreeDecorator());
            ChunkDecorators.Add(new FreezeDecorator());
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

            // TODO: Create a terrain generator initializer function that gets passed the seed etc
            int seed = world.Seed;
            var worley = new CellNoise(seed);
            HighNoise.Seed = seed;
            LowNoise.Seed = seed;
            CaveNoise.Seed = seed;

            var chunk = new Chunk(coordinates);

            for (int x = 0; x < 16; x++)
            {
                for (int z = 0; z < 16; z++)
                {
                    var blockX = MathHelper.ChunkToBlockX(x, coordinates.X);
                    var blockZ = MathHelper.ChunkToBlockZ(z, coordinates.Z);

                    const double lowClampRange = 5;
                    double lowClampMid = LowClamp.MaxValue - ((LowClamp.MaxValue + LowClamp.MinValue) / 2);
                    double lowClampValue = LowClamp.Value2D(blockX, blockZ);

                    if (lowClampValue > lowClampMid - lowClampRange && lowClampValue < lowClampMid + lowClampRange)
                    {
                        InvertNoise NewPrimary = new InvertNoise(HighClamp);
                        FinalNoise.PrimaryNoise = NewPrimary;
                    }
                    else
                    {
                        //reset it after modifying the values
                        FinalNoise = new ModifyNoise(HighClamp, LowClamp, NoiseModifier.Add);
                    }
                    FinalNoise = new ModifyNoise(FinalNoise, BottomClamp, NoiseModifier.Subtract);

                    var cellValue = worley.Value2D(blockX, blockZ);
                    var location = new Coordinates2D(blockX, blockZ);
                    if (world.BiomeDiagram.BiomeCells.Count < 1
                        || cellValue.Equals(1)
                        && world.BiomeDiagram.ClosestCellPoint(location) >= featurePointDistance)
                    {
                        byte id = (SingleBiome) ? GenerationBiome : world.BiomeDiagram.GenerateBiome(seed, Biomes, location);
                        var cell = new BiomeCell(id, location);
                        world.BiomeDiagram.AddCell(cell);
                    }

                    var biomeId = GetBiome(world, location);
                    var biome = Biomes.GetBiome(biomeId);
                    chunk.Biomes[x * Chunk.Width + z] = biomeId;

                    var height = GetHeight(blockX, blockZ);
                    var surfaceHeight = height - biome.SurfaceDepth;
                    chunk.HeightMap[x * Chunk.Width + z] = height;

                    // TODO: Do not overwrite blocks if they are already set from adjacent chunks
                    for (int y = 0; y <= height; y++)
                    {
                        double cave = 0;
                        if (!EnableCaves)
                            cave = double.MaxValue;
                        else
                            cave = CaveNoise.Value3D((blockX + x) / 2, y / 2, (blockZ + z) / 2);
                        double threshold = 0.05;
                        if (y < 4)
                            threshold = double.MaxValue;
                        else
                        {
                            if (y > height - 8)
                                threshold = 8;
                        }
                        if (cave < threshold)
                        {
                            if (y == 0)
                                chunk.SetBlockID(new Coordinates3D(x, y, z), BedrockBlock.BlockID);
                            else
                            {
                                if (y.Equals(height) || y < height && y > surfaceHeight)
                                    chunk.SetBlockID(new Coordinates3D(x, y, z), biome.SurfaceBlock);
                                else
                                {
                                    if (y > surfaceHeight - biome.FillerDepth)
                                        chunk.SetBlockID(new Coordinates3D(x, y, z), biome.FillerBlock);
                                    else
                                        chunk.SetBlockID(new Coordinates3D(x, y, z), StoneBlock.BlockID);
                                }
                            }
                        }
                    }
                }
            }
            foreach (var decorator in ChunkDecorators)
                decorator.Decorate(world, chunk, Biomes);
            chunk.TerrainPopulated = true;
            chunk.UpdateHeightMap();
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
            var NoiseValue = FinalNoise.Value2D(x, z) + GroundLevel;
            if (NoiseValue < 0)
                NoiseValue = GroundLevel;
            if (NoiseValue > Chunk.Height)
                NoiseValue = Chunk.Height - 1;
            return (int)NoiseValue;
        }
    }
}