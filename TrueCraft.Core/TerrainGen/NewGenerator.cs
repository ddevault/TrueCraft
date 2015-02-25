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
        Perlin BottomNoise = new Perlin();
        ClampNoise HighClamp;
        ClampNoise LowClamp;
        ClampNoise BottomClamp;
        ModifyNoise Modified;
        private int GroundLevel = 50;
        public NewGenerator(bool SingleBiome = false, byte GenerateBiome = (byte)Biome.Plains)
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
            int FeaturePointDistance = 400;
            int Seed = world.Seed;
            CellNoise Worley = new CellNoise();
            Worley.Seed = Seed;
            HighNoise.Seed = Seed;
            LowNoise.Seed = Seed;
            var chunk = new Chunk(coordinates);
            for (int X = 0; X < 16; X++)
            {
                for (int Z = 0; Z < 16; Z++)
                {
                    var BlockX = MathHelper.ChunkToBlockX(X, coordinates.X);
                    var BlockZ = MathHelper.ChunkToBlockZ(Z, coordinates.Z);
                    double LowClampMid = LowClamp.MaxValue - ((LowClamp.MaxValue + LowClamp.MinValue) / 2);
                    double LowClampRange = 5;
                    double LowClampValue = LowClamp.Value2D(BlockX, BlockZ);
                    if (LowClampValue > LowClampMid - LowClampRange && LowClampValue < LowClampMid + LowClampRange)
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

                    var Height = GetHeight(BlockX, BlockZ);
                    var SurfaceHeight = Height - Biome.SurfaceDepth;
                    chunk.HeightMap[X * Chunk.Width + Z] = Height;
                    for (int Y = 0; Y <= Height; Y++)
                    {
                        if (Y == 0)
                        {
                            chunk.SetBlockID(new Coordinates3D(X, Y, Z), BedrockBlock.BlockID);
                        }
                        else
                        {
                            if (Y.Equals(Height) || Y < Height && Y > SurfaceHeight)
                            {
                                chunk.SetBlockID(new Coordinates3D(X, Y, Z), Biome.SurfaceBlock);
                            }
                            else
                            {
                                if (Y > SurfaceHeight - Biome.FillerDepth)
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
            var SpawnPointHeight = GetHeight(0, 0);
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

        int GetHeight(int X, int Z)
        {
            var NoiseValue = Modified.Value2D(X, Z) + GroundLevel;
            if (NoiseValue < 0)
                NoiseValue = GroundLevel;
            if (NoiseValue > Chunk.Height)
                NoiseValue = Chunk.Height - 1;
            return (int)NoiseValue;
        }
    }
}