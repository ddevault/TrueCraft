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
    public class DungeonDecorator : IChunkDecorator
    {
        private int BaseLevel;
        public DungeonDecorator(int GroundLevel)
        {
            this.BaseLevel = GroundLevel;
        }

        public void Decorate(IWorld world, IChunk chunk, IBiomeRepository biomes)
        {
            for (int Attemps = 0; Attemps < 8; Attemps++)
            {
                Perlin Noise = new Perlin();
                Noise.Seed = world.Seed - (chunk.Coordinates.X + chunk.Coordinates.Z);
                ClampNoise OffsetNoise = new ClampNoise(Noise);
                OffsetNoise.MaxValue = 3;
                var X = 0;
                var Z = 0;
                double Offset = 0;
                Offset += OffsetNoise.Value2D(X, Z);
                int FinalX = (int)Math.Floor(X + Offset);
                int FinalZ = (int)Math.Floor(Z + Offset);
                IBiomeProvider Biome = biomes.GetBiome(chunk.Biomes[X * Chunk.Width + Z]);
                var Y = (int)(10 + Offset);
                if (FinalX < 6 && FinalZ < 6 && Y < BaseLevel - 10)
                {
                    var BlockX = MathHelper.ChunkToBlockX(X, chunk.Coordinates.X);
                    var BlockZ = MathHelper.ChunkToBlockZ(Z, chunk.Coordinates.Z);
                    var SpawnValue = OffsetNoise.Value2D(BlockX, BlockZ);
                    if (SpawnValue > 1.95 && SpawnValue < 2.09)
                    {
                        GenerateDungeon(world, chunk, new Coordinates3D(FinalX, Y, FinalZ), new Vector3(7, 5, 7), 2);
                        break;
                    }
                }
            }
        }

        void GenerateDungeon(IWorld world, IChunk chunk, Coordinates3D Corner, Vector3 Size, int Chests = 2)
        {
            Perlin Noise = new Perlin();
            Noise.Seed = world.Seed - (chunk.Coordinates.X + chunk.Coordinates.Z);
            int Openings = 0;
            for (int X = Corner.X; X <= Corner.X + Size.X + 1; X++)
            {
                for (int Z = Corner.Z; Z <= Corner.Z + Size.Z + 1; Z++)
                {
                    for (int Y = Corner.Y; Y <= Corner.Y + Size.Y + 1; Y++)
                    {
                        if (Y.Equals(Corner.Y))
                        {
                            if (!IsWall(new Coordinates2D(X, Z), Corner, Size) && !IsCorner(new Coordinates2D(X, Z), Corner, Size))
                            {
                                if (Noise.Value2D(X, Z) > 0.2)
                                {
                                    chunk.SetBlockID(new Coordinates3D(X, Y, Z), MossStoneBlock.BlockID);
                                }
                                else
                                {
                                    chunk.SetBlockID(new Coordinates3D(X, Y, Z), CobblestoneBlock.BlockID);
                                }

                            }
                            else
                            {
                                chunk.SetBlockID(new Coordinates3D(X, Y, Z), CobblestoneBlock.BlockID);
                            }
                        }
                        else if (Y.Equals((int)(Corner.Y + Size.Y + 1)))
                        {
                            chunk.SetBlockID(new Coordinates3D(X, Y, Z), CobblestoneBlock.BlockID);
                        }
                        else
                        {
                            if (IsWall(new Coordinates2D(X, Z), Corner, Size))
                            {
                                if (Noise.Value2D(X, Z) > 0.5 && Openings < 5 && Y.Equals(Corner.Y + 1) && !IsCorner(new Coordinates2D(X, Z), Corner, Size))
                                {
                                    Openings++;
                                }
                                else
                                {
                                    if (!chunk.GetBlockID(new Coordinates3D(X, Y - 1, Z)).Equals(0) && Y <= Corner.Y + 2 || Y > Corner.Y + 2)
                                    {
                                        chunk.SetBlockID(new Coordinates3D(X, Y, Z), CobblestoneBlock.BlockID);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            chunk.SetBlockID(new Coordinates3D((int)(Corner.X + ((Size.X + 1) / 2)), (int)(Corner.Y + 1), (int)(Corner.Z + ((Size.Z + 1) / 2))), MonsterSpawnerBlock.BlockID);
            PlaceChests(Noise, chunk, Corner, Size, Chests);
        }

        void PlaceChests(Perlin DungeonNoise, IChunk chunk, Coordinates3D Corner, Vector3 Size, int Chests)
        {
            ClampNoise ChestNoise = new ClampNoise(DungeonNoise);
            for (int I = 0; I < Chests; I++)
            {
                var X = Corner.X;
                var Z = Corner.Z;
                double Offset = 0;
                for (int Attempts = 0; Attempts < 3; Attempts++)
                {
                    Offset += ChestNoise.Value2D(X, Z);
                    int ChestX = (int)Math.Floor(X + Offset);
                    int ChestZ = (int)Math.Floor(Z + Offset);
                    if (!IsWall(new Coordinates2D(ChestX, ChestZ), Corner, Size))
                    {
                        if (NeighboursBlock(chunk, new Coordinates3D(ChestX, Corner.Y + 1, ChestZ), CobblestoneBlock.BlockID))
                        {
                            chunk.SetBlockID(new Coordinates3D(ChestX, Corner.Y + 1, ChestZ), ChestBlock.BlockID);
                            break;
                        }
                    }
                }
            }
        }

        bool NeighboursBlock(IChunk chunk, Coordinates3D Location, byte Block)
        {
            Coordinates3D Left = new Coordinates3D(Location.X - 1, Location.Y, Location.Z);
            Coordinates3D Right = new Coordinates3D(Location.X + 1, Location.Y, Location.Z);
            Coordinates3D Front = new Coordinates3D(Location.X, Location.Y, Location.Z - 1);
            Coordinates3D Back = new Coordinates3D(Location.X, Location.Y, Location.Z + 1);
            return chunk.GetBlockID(Left).Equals(Block) || chunk.GetBlockID(Right).Equals(Block) || chunk.GetBlockID(Front).Equals(Block) || chunk.GetBlockID(Back).Equals(Block);
        }

        bool IsWall(Coordinates2D Location, Coordinates3D Start, Vector3 Size)
        {
            return Location.X.Equals(Start.X) || Location.Z.Equals(Start.Z) || Location.X.Equals((int)(Start.X + Size.X + 1)) || Location.Z.Equals((int)(Start.Z + Size.Z + 1));
        }

        bool IsCorner(Coordinates2D Location, Coordinates3D Start, Vector3 Size)
        {
            return Location.X.Equals(Start.X - 1) && Location.Z.Equals(Start.Z - 1) || Location.X.Equals((int)(Start.X + Size.X + 1)) && Location.Z.Equals(Start.Z - 1) || Location.Z.Equals(Start.Z - 1) && Location.Z.Equals((int)(Start.Z + Size.Z + 1)) || Location.X.Equals((int)(Start.X + Size.X + 1)) && Location.Z.Equals((int)(Start.Z + Size.Z + 1));
        }
    }
}