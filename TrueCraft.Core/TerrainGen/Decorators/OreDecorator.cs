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
using TrueCraft.API.Logic;

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

        public OreData(byte id, OreTypes type, int minY, int maxY, int viens, int abundance, float rarity)
        {
            ID = id;
            Type = type;
            MinY = minY;
            MaxY = maxY;
            Veins = viens;
            Abundance = abundance;
            Rarity = rarity;
        }
    }

    public class OreDecorator : IChunkDecorator
    {
        private readonly List<OreData> Ores = new List<OreData>();

        public OreDecorator()
        {
            var coal = new OreData(CoalOreBlock.BlockID, OreTypes.Coal, 10, 120, 25, 25, 3f);
            var iron = new OreData(IronOreBlock.BlockID, OreTypes.Iron, 1, 64, 15, 5, 2.3f);
            var lapis = new OreData(LapisLazuliOreBlock.BlockID, OreTypes.Lapiz, 10, 25, 7, 4, 1.4f);
            var gold = new OreData(GoldOreBlock.BlockID, OreTypes.Gold, 1, 32, 6, 4, 1.04f);
            var diamond = new OreData(DiamondOreBlock.BlockID, OreTypes.Diamond, 1, 15, 6, 3, 0.7f);
            var redstone = new OreData(RedstoneOreBlock.BlockID, OreTypes.Redstone, 1, 16, 4, 6, 1.13f);
            Ores.Add(coal);
            Ores.Add(iron);
            Ores.Add(lapis);
            Ores.Add(gold);
            Ores.Add(diamond);
            Ores.Add(redstone);
        }

        public void Decorate(IWorldSeed world, ISpatialBlockInformationProvider chunk, IBiomeRepository biomes, IBlockRepository blockRepository)
        {
            var perlin = new Perlin(world.Seed);
            perlin.Lacunarity = 1;
            perlin.Amplitude = 7;
            perlin.Frequency = 0.015;
            var chanceNoise = new ClampNoise(perlin);
            var noise = new ScaledNoise(perlin);
            var random = new Random(world.Seed);
            var lowWeightOffset = new int[2] { 2, 3 };
            var highWeightOffset = new int[2] { 2, 2 };
            foreach (var data in Ores)
            {
                var midpoint = (data.MaxY + data.MinY) / 2;
                var weightOffsets = (data.MaxY > 30) ? highWeightOffset : lowWeightOffset;
                const int weightPasses = 4;
                for (int i = 0; i < data.Veins; i++)
                {
                    double weight = 0;
                    for (int j = 0; j < weightPasses; j++)
                    {
                        weight += random.NextDouble();
                    }
                    weight /= data.Rarity;
                    weight = weightOffsets[0] - Math.Abs(weight - weightOffsets[1]);
                    double x = random.Next(0, Chunk.Width);
                    double z = random.Next(0, Chunk.Depth);
                    double y = weight * midpoint;

                    double randomOffsetX = (float)random.NextDouble() - 1;
                    double randomOffsetY = (float)random.NextDouble() - 1;
                    double randomOffsetZ = (float)random.NextDouble() - 1;

                    int abundance = random.Next(0, data.Abundance);
                    for (int k = 0; k < abundance; k++)
                    {
                        x += randomOffsetX;
                        y += randomOffsetY;
                        z += randomOffsetZ;
                        if (x >= 0 && z >= 0 && y >= data.MinY && x < Chunk.Width && y < data.MaxY && z < Chunk.Depth)
                        {
                            var biome = biomes.GetBiome(chunk.Biomes[(int)(x * Chunk.Width + z)]);
                            if (biome.Ores.Contains(data.Type) && chunk.GetBlockID(new Coordinates3D((int)x, (int)y, (int)z)).Equals(StoneBlock.BlockID))
                                chunk.SetBlockID(new Coordinates3D((int)x, (int)y, (int)z), data.ID);
                        }
                        var blockX = MathHelper.ChunkToBlockX((int)(x), chunk.Coordinates.X);
                        var blockZ = MathHelper.ChunkToBlockZ((int)(z), chunk.Coordinates.Z);

                        double offsetX = 0;
                        double offsetY = 0;
                        double offsetZ = 0;
                        int offset = random.Next(0, 3);
                        double offset2 = random.NextDouble();

                        if (offset.Equals(0) && offset2 < 0.4)
                            offsetX += 1;
                        else if (offset.Equals(1) && offset2 >= 0.4 && offset2 < 0.65)
                            offsetY += 1;
                        else
                            offsetZ += 1;

                        var newX = (int)(x + offsetX);
                        var newY = (int)(y + offsetY);
                        var newZ = (int)(z + offsetZ);
                        if (newX >= 0 && newZ >= 0 && newY >= data.MinY && newX < Chunk.Width && newY < data.MaxY && newZ < Chunk.Depth)
                        {
                            IBiomeProvider Biome = biomes.GetBiome(chunk.Biomes[newX * Chunk.Width + newZ]);
                            var coordinates = new Coordinates3D((int)newX, (int)newY, (int)newZ);
                            if (Biome.Ores.Contains(data.Type) && chunk.GetBlockID(coordinates).Equals(StoneBlock.BlockID))
                            {
                                chunk.SetBlockID(coordinates, data.ID);
                            }
                        }
                    }
                }
            }
        }
    }
}