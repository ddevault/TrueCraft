using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.World;
using TrueCraft.API;
using TrueCraft.Core.World;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.TerrainGen.Decorators
{
    public class WaterDecorator : IChunkDecorator
    {
        int WaterLevel = 40;
        public void Decorate(IWorld world, IChunk chunk, IBiomeRepository Biomes)
        {
            for (int X = 0; X < Chunk.Width; X++)
            {
                for (int Z = 0; Z < Chunk.Depth; Z++)
                {
                    IBiomeProvider Biome = Biomes.GetBiome(chunk.Biomes[X * Chunk.Width + Z]);
                    var Height = chunk.HeightMap[X * Chunk.Width + Z];
                    for (int Y = Height; Y <= WaterLevel; Y++)
                    {
                        Coordinates3D BlockLocation = new Coordinates3D(X, Y, Z);
                        int BlockID = chunk.GetBlockID(BlockLocation);
                        if (BlockID.Equals(AirBlock.BlockID))
                        {
                            chunk.SetBlockID(BlockLocation, Biome.WaterBlock);
                            Coordinates3D Below = BlockLocation + Coordinates3D.Down;
                            if (!chunk.GetBlockID(Below).Equals(AirBlock.BlockID) && !chunk.GetBlockID(Below).Equals(Biome.WaterBlock))
                            {
                                if (!Biome.WaterBlock.Equals(LavaBlock.BlockID) && !Biome.WaterBlock.Equals(StationaryLavaBlock.BlockID))
                                {
                                    Random R = new Random(world.Seed);
                                    if (R.Next(100) < 40)
                                    {
                                        chunk.SetBlockID(Below, ClayBlock.BlockID);
                                    }
                                    else
                                    {
                                        chunk.SetBlockID(Below, SandBlock.BlockID);
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