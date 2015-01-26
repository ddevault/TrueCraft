using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.World;
using TrueCraft.API;
using TrueCraft.Core.World;
using TrueCraft.Core.Logic.Blocks;
using LibNoise;
using LibNoise.Models;

namespace TrueCraft.Core.TerrainGen
{
    /// <summary>
    /// This terrain generator is for testing purposes only. Use at your own risk.
    /// </summary>
    public class TestGenerator : IChunkProvider
    {
        int GroundLevel = 35;
        public TestGenerator()
        {
            ChunkDecorators = new List<IChunkDecorator>();
        }

        public IList<IChunkDecorator> ChunkDecorators { get; set; }
        public Vector3 SpawnPoint { get; private set; }

        public IChunk GenerateChunk(IWorld world, Coordinates2D coordinates)
        {
            var chunk = new Chunk(coordinates);

            int[] Heights = new int[256];
            int[] BiomeData = new int[256];
            for (int X = 0; X < 16; X++)
            {
                for (int Z = 0; Z < 16; Z++)
                {
                    int Height = GroundLevel + (int)GenerateHeight(chunk.X * Chunk.Width + X, chunk.Z * Chunk.Depth + Z, 4.0f);
                    if (X == 0 && Z == 0)
                    {
                        SpawnPoint = new Vector3(X, Height + 2, Z);
                    }
                    for (int Y = 0; Y <= Height; Y++)
                    {
                        if (Y == Height)
                        {
                            chunk.SetBlockID(new Coordinates3D(X, Y, Z), GrassBlock.BlockID);
                        }
                        else if (Y == 0)
                        {
                            chunk.SetBlockID(new Coordinates3D(X, Y, Z), BedrockBlock.BlockID);
                        }
                        else if (Y < Height)
                        {
                            chunk.SetBlockID(new Coordinates3D(X, Y, Z), DirtBlock.BlockID);
                        }
                    }
                    Heights[X * 16 + Z] = Height;
                    BiomeData[X * 16 + Z] = new Random().Next(1, 6);
                }
            }
            foreach (IChunkDecorator ChunkDecorator in ChunkDecorators)
            {
                ChunkDecorator.Decorate(chunk);
            }
            return chunk;
        }

        public float GenerateHeight(int X, int Z, float Modifier = 1.0f)
        {
            Plane PlaneNoise = new Plane(new Perlin());
            return (float)PlaneNoise.GetValue(Modifier * X / new Random().Next(1, 16), Modifier * Z / new Random().Next(1, 16));
        }
    }
}