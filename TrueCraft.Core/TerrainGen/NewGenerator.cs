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

namespace TrueCraft.Core.TerrainGen
{
    /// <summary>
    /// This terrain generator is still under heavy development. Use at your own risk.
    /// </summary>
    public class NewGenerator : IChunkProvider
    {
        BiomeRepository Biomes = new BiomeRepository();
        Perlin HighNoise = new Perlin();
        Perlin LowNoise = new Perlin();
        ClampNoise HighClamp;
        ClampNoise LowClamp;
        private int GroundLevel = 50;
        public NewGenerator(bool SingleBiome = false, byte GenerateBiome = (byte)Biome.Plains)
        {
            this.SingleBiome = SingleBiome;
            this.GenerationBiome = GenerateBiome;
            HighNoise.Persistance = 1;
            HighNoise.Frequency = 0.023;
            HighNoise.Amplitude = 14;
            HighNoise.Octaves = 2;
            HighNoise.Lacunarity = 2;
            LowNoise.Persistance = 1;
            LowNoise.Frequency = 0.023;
            LowNoise.Amplitude = 5;
            LowNoise.Octaves = 2;
            LowNoise.Lacunarity = 2;
            HighClamp = new ClampNoise(HighClamp);
            HighClamp.MinValue = 0;
            HighClamp.MaxValue = 20;
            LowClamp = new ClampNoise(LowNoise);
            LowClamp.MinValue = -20;
            LowClamp.MaxValue = 20;
            ChunkDecorators = new List<IChunkDecorator>();
            ChunkDecorators.Add(new WaterDecorator());
            ChunkDecorators.Add(new TreeDecorator());
            ChunkDecorators.Add(new FreezeDecorator());
            ChunkDecorators.Add(new PlantDecorator());
            ChunkDecorators.Add(new CactusDecorator());
            ChunkDecorators.Add(new SugarCaneDecorator());
            ChunkDecorators.Add(new OreDecorator());
            ChunkDecorators.Add(new DungeonDecorator(GroundLevel));
        }

        public IList<IChunkDecorator> ChunkDecorators { get; private set; }
        public Vector3 SpawnPoint { get; private set; }
        public bool SingleBiome { get; private set; }
        public byte GenerationBiome { get; private set; }
        public IChunk GenerateChunk(IWorld world, Coordinates2D coordinates)
        {
            int FeaturePointDistance = 90;
            CellNoise Worley = new CellNoise();
            Worley.Seed = world.Seed;
            int Seed = world.Seed;
            HighNoise.Seed = Seed;
            LowNoise.Seed = Seed;
            ModifyNoise Modified = new ModifyNoise(HighNoise, LowNoise, NoiseModifier.Add);
            var chunk = new Chunk(coordinates);
            for (int X = 0; X < 16; X++)
            {
                for (int Z = 0; Z < 16; Z++)
                {
                    var BlockX = MathHelper.ChunkToBlockX(X, coordinates.X);
                    var BlockZ = MathHelper.ChunkToBlockZ(Z, coordinates.Z);
                    var CellValue = Worley.Value2D(BlockX, BlockZ);
                    var Location = new Coordinates2D(BlockX, BlockZ);
                    if (world.BiomeDiagram.BiomeCells.Count < 1 || CellValue.Equals(1) && world.BiomeDiagram.ClosestCellPoint(Location) >= FeaturePointDistance)
                    {
                        byte ID = (SingleBiome) ? GenerationBiome : world.BiomeDiagram.GenerateBiome(Seed, Biomes, Location);
                        BiomeCell Cell = new BiomeCell(ID, Location);
                        world.BiomeDiagram.AddCell(Cell);
                    }

                    var BiomeID = GetBiome(world, Location);
                    IBiomeProvider Biome = Biomes.GetBiome(BiomeID);
                    chunk.Biomes[X * Chunk.Width + Z] = BiomeID;

                    var Height = GetHeight(BlockX, BlockZ, Modified);
                    chunk.HeightMap[X * Chunk.Width + Z] = Height;
                    for (int Y = 0; Y <= Height; Y++)
                    {
                        if (Y == 0)
                        {
                            chunk.SetBlockID(new Coordinates3D(X, Y, Z), BedrockBlock.BlockID);
                        }
                        else
                        {
                            if (Y.Equals(Height))
                            {
                                chunk.SetBlockID(new Coordinates3D(X, Y, Z), Biome.SurfaceBlock);
                            }
                            else
                            {
                                if (Y < Height && Y > Height - 4)
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
            {
                ChunkDecorator.Decorate(world, chunk, Biomes);
            }
            var SpawnOffset = 2;
            var SpawnPointHeight = GetHeight(0, 0, Modified);
            if (SpawnPointHeight + SpawnOffset < Chunk.Height)
                SpawnPointHeight += SpawnOffset;
            SpawnPoint = new Vector3(0, SpawnPointHeight, 0);
            return chunk;
        }

        byte GetBiome(IWorld world, Coordinates2D Location)
        {
            if (SingleBiome)
            {
                return GenerationBiome;
            }
            return world.BiomeDiagram.GetBiome(Location);
        }

        int GetHeight(int X, int Z, INoise Noise)
        {
            var NoiseValue = Noise.Value2D(X, Z) + GroundLevel;
            if (NoiseValue < 0)
                NoiseValue = GroundLevel;
            if (NoiseValue > Chunk.Height)
                NoiseValue = Chunk.Height - 1;
            return (int)NoiseValue;
        }
    }
}