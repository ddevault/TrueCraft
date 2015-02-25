using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.World;
using TrueCraft.Core.World;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.API;

namespace TrueCraft.Core.TerrainGen.Decorators
{
    class FreezeDecorator : IChunkDecorator
    {
        public void Decorate(IWorld world, IChunk chunk, IBiomeRepository biomes)
        {
            for (int X = 0; X < 16; X++)
            {
                for (int Z = 0; Z < 16; Z++)
                {
                    IBiomeProvider Biome = biomes.GetBiome(chunk.Biomes[X * Chunk.Width + Z]);
                    if (Biome.Temperature < 0.15)
                    {
                        var Height = chunk.HeightMap[X * Chunk.Width + Z];
                        for (int Y = Height; Y < Chunk.Height; Y++)
                        {
                            Coordinates3D Location = new Coordinates3D(X, Y, Z);
                            if (chunk.GetBlockID(Location).Equals(StationaryWaterBlock.BlockID) || chunk.GetBlockID(Location).Equals(WaterBlock.BlockID))
                            {
                                chunk.SetBlockID(Location, IceBlock.BlockID);
                            }
                            else
                            {
                                if (chunk.GetBlockID(Location).Equals(IceBlock.BlockID) && CoverIce(chunk, biomes, Location))
                                {
                                    chunk.SetBlockID((Location + Coordinates3D.Up), SnowfallBlock.BlockID);
                                }
                                else if (!chunk.GetBlockID(Location).Equals(SnowfallBlock.BlockID) && !chunk.GetBlockID(Location).Equals(AirBlock.BlockID))
                                {
                                    chunk.SetBlockID((Location + Coordinates3D.Up), SnowfallBlock.BlockID);
                                }
                            }
                        }
                    }
                }
            }
        }

        bool CoverIce(IChunk chunk, IBiomeRepository biomes, Coordinates3D Location)
        {
            var MaxDistance = 4;
            var Surrounding = new[] {
                Location + new Coordinates3D(-MaxDistance, 0, 0),
                Location + new Coordinates3D(MaxDistance, 0, 0),
                Location + new Coordinates3D(0, 0, MaxDistance),
                Location + new Coordinates3D(0, 0, -MaxDistance),
            };
            for (int I = 0; I < Surrounding.Length; I++)
            {
                Coordinates3D Check = Surrounding[I];
                if (Check.X < 0 || Check.X >= Chunk.Width || Check.Z < 0 || Check.Z >= Chunk.Depth || Check.Y < 0 || Check.Y >= Chunk.Height)
                    return false;
                IBiomeProvider Biome = biomes.GetBiome(chunk.Biomes[Check.X * Chunk.Width + Check.Z]);
                if (chunk.GetBlockID(Check).Equals(Biome.SurfaceBlock) || chunk.GetBlockID(Check).Equals(Biome.FillerBlock))
                    return true;
            }
            return false;
        }
    }
}