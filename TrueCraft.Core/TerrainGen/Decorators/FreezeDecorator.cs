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
                            if (Height < Chunk.Height - 1)
                            {
                                if (chunk.GetBlockID(new Coordinates3D(X, Height, Z)).Equals(StationaryWaterBlock.BlockID))
                                {
                                    chunk.SetBlockID(new Coordinates3D(X, Height, Z), IceBlock.BlockID);
                                }
                                else
                                {
                                    if (!chunk.GetBlockID(new Coordinates3D(X, Height, Z)).Equals(SnowfallBlock.BlockID) && !chunk.GetBlockID(new Coordinates3D(X, Height, Z)).Equals(0))
                                    {
                                        chunk.SetBlockID(new Coordinates3D(X, Height + 1, Z), SnowfallBlock.BlockID);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}