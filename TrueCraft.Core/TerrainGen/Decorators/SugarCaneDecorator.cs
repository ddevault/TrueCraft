using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.World;
using TrueCraft.Core.World;
using TrueCraft.Core.TerrainGen.Noise;
using TrueCraft.API;
using TrueCraft.Core.Logic.Blocks;

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
                            Coordinates3D StalkStart = BlockLocation + Coordinates3D.Up;
                            if (chunk.GetBlockID(BlockLocation).Equals(GrassBlock.BlockID) && NeighboursWater(chunk, BlockLocation) || chunk.GetBlockID(BlockLocation).Equals(SandBlock.BlockID) && NeighboursWater(chunk, BlockLocation))
                            {
                                PlaceStalk(chunk, StalkStart, world.Seed);
                            }
                        }
                    }
                }
            }
        }

        void PlaceStalk(IChunk chunk, Coordinates3D location, int seed)
        {
            Random R = new Random(seed);
            double HeightChance = R.NextDouble();
            int Height = 3;
            if (HeightChance < 0.05)
            {
                Height = 4;
            }
            else if (HeightChance > 0.1 && Height < 0.25)
            {
                Height = 2;
            }
            Coordinates3D NewLocation = location;
            for (int Y = location.Y; Y < location.Y + Height; Y++)
            {
                NewLocation.Y = Y;
                chunk.SetBlockID(NewLocation, SugarcaneBlock.BlockID);
            }
        }

        bool NeighboursWater(IChunk chunk, Coordinates3D location)
        {
            var Surrounding = new[] {
                location + Coordinates3D.Left,
                location + Coordinates3D.Right,
                location + Coordinates3D.Forwards,
                location + Coordinates3D.Backwards,
            };
            for (int I = 0; I < Surrounding.Length; I++)
            {
                Coordinates3D Check = Surrounding[I];
                if (Check.X < 0 || Check.X >= Chunk.Width || Check.Z < 0 || Check.Z >= Chunk.Depth || Check.Y < 0 || Check.Y >= Chunk.Height)
                    return false;
                if (chunk.GetBlockID(Check).Equals(WaterBlock.BlockID) || chunk.GetBlockID(Check).Equals(StationaryWaterBlock.BlockID))
                    return true;
            }
            return false;
        }
    }
}