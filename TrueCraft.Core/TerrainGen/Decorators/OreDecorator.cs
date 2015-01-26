using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.World;
using TrueCraft.Core.TerrainGen.Noise;
using TrueCraft.Core.World;
using TrueCraft.API;
using TrueCraft.Core.Logic.Blocks;
using System.IO;

namespace TrueCraft.Core.TerrainGen.Decorators
{
    public struct OreData
    {
        public byte ID;
        public OreTypes Type;
        public int MinY;
        public int MaxY;
        public int Veins;
        public int Abundance;
        public float Rarity;

        public OreData(byte ID, OreTypes Type, int MinY, int MaxY, int Veins, int Abundance, float Rarity)
        {
            this.ID = ID;
            this.Type = Type;
            this.MinY = MinY;
            this.MaxY = MaxY;
            this.Veins = Veins;
            this.Abundance = Abundance;
            this.Rarity = Rarity;
        }
    }

    public class OreDecorator : IChunkDecorator
    {
        private List<OreData> Ores = new List<OreData>();

        public OreDecorator()
        {
            OreData Coal = new OreData(CoalOreBlock.BlockID, OreTypes.Coal, 10, 120, 25, 25, 3f);
            OreData Iron = new OreData(IronOreBlock.BlockID, OreTypes.Iron, 1, 64, 15, 5, 2.3f);
            OreData Lapis = new OreData(LapisLazuliOreBlock.BlockID, OreTypes.Lapiz, 10, 25, 7, 4, 1.4f);
            OreData Gold = new OreData(GoldOreBlock.BlockID, OreTypes.Gold, 1, 32, 6, 4, 1.04f);
            OreData Diamond = new OreData(DiamondOreBlock.BlockID, OreTypes.Diamond, 1, 15, 6, 3, 0.7f);
            OreData Redstone = new OreData(RedstoneOreBlock.BlockID, OreTypes.Redstone, 1, 16, 4, 6, 1.13f);
            Ores.Add(Coal);
            Ores.Add(Iron);
            Ores.Add(Lapis);
            Ores.Add(Gold);
            Ores.Add(Diamond);
            Ores.Add(Redstone);
        }

        public void Decorate(IWorld world, IChunk chunk, IBiomeRepository biomes)
        {
            //Test Seed: 291887241
            Perlin Perlin = new Perlin();
            Perlin.Lacunarity = 1;
            Perlin.Amplitude = 7;
            Perlin.Frequency = 0.015;
            Perlin.Seed = world.Seed;
            ClampNoise ChanceNoise = new ClampNoise(Perlin);
            ScaledNoise Noise = new ScaledNoise(Perlin);
            Random R = new Random(world.Seed);
            var LowWeightOffset = new int[2] { 2, 3 };
            var HighWeightOffset = new int[2] { 2, 2 };
            foreach (OreData Data in Ores)
            {
                var Midpoint = (Data.MaxY + Data.MinY) / 2;
                var WeightOffsets = (Data.MaxY > 30) ? HighWeightOffset : LowWeightOffset;
                var WeightPasses = 4;
                for (int I = 0; I < Data.Veins; I++)
                {
                    double Weight = 0;
                    for (int J = 0; J < WeightPasses; J++)
                    {
                        Weight += R.NextDouble();
                    }
                    Weight /= Data.Rarity;
                    Weight = WeightOffsets[0] - Math.Abs(Weight - WeightOffsets[1]);
                    double X = R.Next(0, Chunk.Width);
                    double Z = R.Next(0, Chunk.Depth);
                    double Y = Weight * Midpoint;

                    double RandomOffX = (float)R.NextDouble() - 1;
                    double RandomOffY = (float)R.NextDouble() - 1;
                    double RandomOffZ = (float)R.NextDouble() - 1;

                    int Abundance = R.Next(0, Data.Abundance);
                    for (int K = 0; K < Abundance; K++)
                    {
                        X += RandomOffX;
                        Y += RandomOffY;
                        Z += RandomOffZ;
                        if (X >= 0 && Z >= 0 && Y >= Data.MinY && X < Chunk.Width && Y < Data.MaxY && Z < Chunk.Depth)
                        {
                            IBiomeProvider Biome = biomes.GetBiome(chunk.Biomes[(int)(X * Chunk.Width + Z)]);
                            if (Biome.Ores.Contains(Data.Type) && chunk.GetBlockID(new Coordinates3D((int)X, (int)Y, (int)Z)).Equals(GlassBlock.BlockID))
                            {
                                chunk.SetBlockID(new Coordinates3D((int)X, (int)Y, (int)Z), Data.ID);
                            }
                        }
                        var BlockX = MathHelper.ChunkToBlockX((int)(X), chunk.Coordinates.X);
                        var BlockZ = MathHelper.ChunkToBlockZ((int)(Z), chunk.Coordinates.Z);

                        double OffX = 0;
                        double OffY = 0;
                        double OffZ = 0;
                        int Off = R.Next(0, 3);
                        double Off2 = R.NextDouble();
                        if (Off.Equals(0) && Off2 < 0.4)
                        {
                            OffX += 1;
                        }
                        else if (Off.Equals(1) && Off2 >= 0.4 && Off2 < 0.65)
                        {
                            OffY += 1;
                        }
                        else
                        {
                            OffZ += 1;
                        }
                        var NewX = (int)(X + OffX);
                        var NewY = (int)(Y + OffY);
                        var NewZ = (int)(Z + OffZ);
                        if (NewX >= 0 && NewZ >= 0 && NewY >= Data.MinY && NewX < Chunk.Width && NewY < Data.MaxY && NewZ < Chunk.Depth)
                        {
                            IBiomeProvider Biome = biomes.GetBiome(chunk.Biomes[NewX * Chunk.Width + NewZ]);
                            if (Biome.Ores.Contains(Data.Type) && chunk.GetBlockID(new Coordinates3D((int)NewX, (int)NewY, (int)NewZ)).Equals(GlassBlock.BlockID))
                            {
                                chunk.SetBlockID(new Coordinates3D((int)NewX, (int)NewY, (int)NewZ), Data.ID);
                            }
                        }
                    }
                }
            }
        }
    }
}