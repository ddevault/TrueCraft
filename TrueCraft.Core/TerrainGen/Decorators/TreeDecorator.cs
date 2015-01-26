using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.World;
using TrueCraft.API;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.Core.TerrainGen.Noise;
using TrueCraft.Core.World;

namespace TrueCraft.Core.TerrainGen.Decorators
{
    public class TreeDecorator : IChunkDecorator
    {
        Perlin Noise;
        ClampNoise ChanceNoise;
        public void Decorate(IWorld world, IChunk chunk, IBiomeRepository biomes)
        {
            Noise = new Perlin();
            Noise.Seed = world.Seed;
            ChanceNoise = new ClampNoise(Noise);
            ChanceNoise.MaxValue = 2;
            Coordinates2D LastTree = new Coordinates2D();
            for (int X = 0; X < 16; X++)
            {
                for (int Z = 0; Z < 16; Z++)
                {
                    IBiomeProvider Biome = biomes.GetBiome(chunk.Biomes[X * Chunk.Width + Z]);
                    var BlockX = MathHelper.ChunkToBlockX(X, chunk.Coordinates.X);
                    var BlockZ = MathHelper.ChunkToBlockZ(Z, chunk.Coordinates.Z);
                    var Height = chunk.HeightMap[X * Chunk.Width + Z] + 1;

                    if (!LastTree.Equals(null) && LastTree.DistanceTo(new Coordinates2D(X, Z)) < Biome.TreeDensity)
                    {
                        continue;
                    }

                    if (Noise.Value2D(BlockX, BlockZ) > 0.3)
                    {
                        if (chunk.GetBlockID(new Coordinates3D(X, Height - 1, Z)).Equals(GrassBlock.BlockID))
                        {
                            var Chance = ChanceNoise.Value2D(BlockX, BlockZ);
                            var OakNoise = ChanceNoise.Value2D(BlockX * 0.6, BlockZ * 0.6);
                            var BirchNoise = ChanceNoise.Value2D(BlockX * 0.2, BlockZ * 0.2);
                            var SpruceNoise = ChanceNoise.Value2D(BlockX * 0.35, BlockZ * 0.35);
                            //We currently have to limit where trees can spawn in the trunk due to not being able to use other chunks during generation.
                            if (X + 4 > 15 || Z + 4 > 15 || X - 4 < 0 || Z - 4 < 0)
                                continue;
                            if (Biome.Trees.Contains(TreeSpecies.Oak) && OakNoise > 1.01 && OakNoise < 1.25)
                            {
                                GenerateTree(chunk, new Coordinates3D(X, Height, Z), TreeSpecies.Oak);
                                LastTree = new Coordinates2D(X, Z);
                                continue;
                            }
                            if (Biome.Trees.Contains(TreeSpecies.Birch) && BirchNoise > 0.3 && BirchNoise < 0.95)
                            {
                                GenerateTree(chunk, new Coordinates3D(X, Height, Z), TreeSpecies.Birch);
                                LastTree = new Coordinates2D(X, Z);
                                continue;
                            }
                            if (Biome.Trees.Contains(TreeSpecies.Spruce) && SpruceNoise < 0.75)
                            {
                                if (X + 3 > 15 || Z + 3 > 15 || X - 3 < 0 || Z - 3 < 0)
                                    continue;
                                GenerateTree(chunk, new Coordinates3D(X, Height, Z), TreeSpecies.Spruce);
                                LastTree = new Coordinates2D(X, Z);
                                continue;
                            }
                        }
                    }
                }
            }
        }

        private void GenerateTree(IChunk chunk, Coordinates3D Base, TreeSpecies TreeType)
        {
            switch (TreeType)
            {
                case TreeSpecies.Oak:
                    GenerateOak(chunk, Base, 0x0, 0x0);
                    break;
                case TreeSpecies.Birch:
                    GenerateOak(chunk, Base, 0x2, 0x2);
                    break;
                case TreeSpecies.Spruce:
                    GenerateSpruce(chunk, Base, 0x1, 0x1);
                    break;
                default:
                    GenerateOak(chunk, Base, 0x0, 0x0);
                    break;
            }
        }

        private void GenerateOak(IChunk chunk, Coordinates3D Base, byte Logs, byte Leaves, OakType Type = OakType.Normal)
        {
            var BlockX = MathHelper.ChunkToBlockX(Base.X, chunk.Coordinates.X);
            var BlockZ = MathHelper.ChunkToBlockZ(Base.Z, chunk.Coordinates.Z);
            var HeightChance = ChanceNoise.Value2D(BlockX, BlockZ);
            var TreeHeight = (HeightChance < 1.2) ? 4 : 5;
            int Radius = 2;
            if (Type.Equals(OakType.Normal))
            {
                int OffRadius = Radius;
                int Offset = 1;
                for (int Y = Base.Y; Y < Base.Y + TreeHeight + 2; Y++)
                {
                    if (Y - Base.Y <= TreeHeight)
                    {
                        chunk.SetBlockID(new Coordinates3D(Base.X, Y, Base.Z), WoodBlock.BlockID);
                        chunk.SetMetadata(new Coordinates3D(Base.X, Y, Base.Z), Logs);
                    }
                    if (Y - Base.Y >= TreeHeight - Radius)
                    {
                        GenerateCircleLeaves(chunk, new Coordinates3D(Base.X, Y, Base.Z), OffRadius, Leaves, ChanceNoise.Value2D((BlockX / Y) * 0.25, (BlockZ / Y) * 0.25));
                        if (Offset % 2 == 0)
                            OffRadius--;
                        Offset++;
                    }
                }
            }
            else if (Type.Equals(OakType.BalloonBlocky))
            {
                int OffRadius = 1;//Start at 1 
                int Offset = 1;
                for (int Y = Base.Y; Y < Base.Y + TreeHeight + 3; Y++)
                {
                    if (Y - Base.Y <= TreeHeight)
                    {
                        chunk.SetBlockID(new Coordinates3D(Base.X, Y, Base.Z), WoodBlock.BlockID);
                        chunk.SetMetadata(new Coordinates3D(Base.X, Y, Base.Z), Logs);
                    }
                    if (Y - Base.Y >= TreeHeight - Radius)
                    {
                        GenerateCircleLeaves(chunk, new Coordinates3D(Base.X, Y, Base.Z), OffRadius, Leaves);
                        if ((Offset - 1) % 3 == 0 && OffRadius.Equals(2) || Offset % 1 == 0 && OffRadius.Equals(1))
                        {
                            if (Y - Base.Y >= TreeHeight)
                            {
                                OffRadius--;
                            }
                            else
                            {
                                OffRadius++;
                            }
                        }
                        Offset++;
                    }
                }
            }
            else if (Type.Equals(OakType.Balloon))
            {
                for (int Y = Base.Y; Y < Base.Y + TreeHeight; Y++)
                {
                    if (Y - Base.Y <= TreeHeight)
                    {
                        chunk.SetBlockID(new Coordinates3D(Base.X, Y, Base.Z), WoodBlock.BlockID);
                        chunk.SetMetadata(new Coordinates3D(Base.X, Y, Base.Z), Logs);
                    }
                    GenerateSphereLeaves(chunk, new Coordinates3D(Base.X, Base.Y + TreeHeight, Base.Z), Radius, Leaves);
                }
            }
            //TODO: I don't want to implement branched oak trees until we're able to use other chunks during generation
        }

        private void GenerateSpruce(IChunk chunk, Coordinates3D Base, byte Logs, byte Leaves)
        {
            var BlockX = MathHelper.ChunkToBlockX(Base.X, chunk.Coordinates.X);
            var BlockZ = MathHelper.ChunkToBlockZ(Base.Z, chunk.Coordinates.Z);
            var HeightChance = ChanceNoise.Value2D(BlockX, BlockZ);
            var TreeHeight = (HeightChance < 1.4) ? 7 : 8;
            var Type = (HeightChance < 1.4 && HeightChance > 0.9) ? 2 : 1;

            int SeperatorRadius = 1;
            int Radius = 2;
            int Offset = (HeightChance > 0.4 && HeightChance < 0.6) ? 1 : 0;
            for (int Y = Base.Y; Y < Base.Y + TreeHeight + 4; Y++)
            {
                if (Y - Base.Y <= TreeHeight)
                {
                    chunk.SetBlockID(new Coordinates3D(Base.X, Y, Base.Z), WoodBlock.BlockID);
                    chunk.SetMetadata(new Coordinates3D(Base.X, Y, Base.Z), Logs);
                }
                if (Y - Base.Y >= TreeHeight - 5)
                {
                    int CurrentRadius = ((Offset % 2).Equals(0)) ? Radius : SeperatorRadius;
                    if (Offset.Equals(0) && Type.Equals(2))
                        CurrentRadius = 3;
                    if (Y.Equals(Base.Y + TreeHeight + 1) || Y.Equals(Base.Y + TreeHeight + 3))
                        CurrentRadius = SeperatorRadius;
                    if (Y.Equals(Base.Y + TreeHeight + 2))
                    {
                        Coordinates3D Block = new Coordinates3D(Base.X, Y, Base.Z);
                        chunk.SetBlockID(Block, LeavesBlock.BlockID);
                        chunk.SetMetadata(Block, Leaves);
                        continue;
                    }
                    GenerateCircleLeaves(chunk, new Coordinates3D(Base.X, Y, Base.Z), CurrentRadius, Leaves);
                    Offset++;
                }
            }
        }

        private void GenerateCircleLeaves(IChunk chunk, Coordinates3D MiddleBlock, int Radius, byte Leaves, double CornerChance = 0)
        {
            for (int I = -Radius; I <= Radius; I = (I + 1))
            {
                for (int J = -Radius; J <= Radius; J = (J + 1))
                {
                    int Max = (int)Math.Sqrt((I * I) + (J * J));
                    if (Max <= Radius)
                    {
                        if (I.Equals(-Radius) && J.Equals(-Radius) || I.Equals(-Radius) && J.Equals(Radius) || I.Equals(Radius) && J.Equals(-Radius) || I.Equals(Radius) && J.Equals(Radius))
                        {
                            if (CornerChance + Radius * 0.2 < 0.4 || CornerChance + Radius * 0.2 > 0.7 || CornerChance.Equals(0))
                                continue;
                        }
                        int X = MiddleBlock.X + I;
                        int Z = MiddleBlock.Z + J;
                        Coordinates3D CurrentBlock = new Coordinates3D(X, MiddleBlock.Y, Z);
                        if (chunk.GetBlockID(CurrentBlock).Equals(0))
                        {
                            chunk.SetBlockID(CurrentBlock, LeavesBlock.BlockID);
                            chunk.SetMetadata(CurrentBlock, Leaves);
                        }
                    }
                }
            }
        }

        private void GenerateSphereLeaves(IChunk chunk, Coordinates3D MiddleBlock, int Radius, byte Leaves)
        {
            for (int I = -Radius; I <= Radius; I = (I + 1))
            {
                for (int J = -Radius; J <= Radius; J = (J + 1))
                {
                    for (int K = -Radius; K <= Radius; K = (K + 1))
                    {
                        int Max = (int)Math.Sqrt((I * I) + (J * J) + (K * K));
                        if (Max <= Radius)
                        {
                            int X = MiddleBlock.X + I;
                            int Y = MiddleBlock.Y + K;
                            int Z = MiddleBlock.Z + J;
                            Coordinates3D CurrentBlock = new Coordinates3D(X, Y, Z);
                            if (chunk.GetBlockID(CurrentBlock).Equals(0))
                            {
                                chunk.SetBlockID(CurrentBlock, LeavesBlock.BlockID);
                                chunk.SetMetadata(CurrentBlock, Leaves);
                            }
                        }
                    }
                }
            }
        }
    }
}