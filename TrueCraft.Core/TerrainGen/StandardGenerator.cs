using System;
using System.Collections.Generic;
using System.Collections;
using TrueCraft.API.World;
using TrueCraft.API;
using TrueCraft.Core.World;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.TerrainGen
{
    // This terrain generator is very basic, non-deterministic, and needs a lot of work
    // It'll probably have to be thrown out and done from scratch later
    public class StandardGenerator : IChunkProvider
    {
        /// <summary>
        /// The noise generator used
        /// </summary>
        private Noise noise;
        /// <summary>
        /// The water level.
        /// </summary>
        private int waterLevel = 50;

        public StandardGenerator()
        {
            const double persistence = 1, frequency = 0.01, amplitude = 80;
            int octaves = 2;
            noise = new Noise(persistence, frequency, amplitude, octaves, new Random().Next());
            SpawnPoint = new Vector3(0, GetHeight(0, 0) + 1, 0);
        }

        public Vector3 SpawnPoint { get; private set; }

        /// <summary>
        /// Generates a chunk by getting an array of heights then placing blocks of varying types up to that height
        /// then it adds trees (leaves first then trunk)
        /// 
        /// </summary>
        /// <returns>The chunk.</returns>
        /// <param name="position">Position.</param>
        public IChunk GenerateChunk(IWorld world, Coordinates2D position)
        {
            // TODO: Add Ores
            // TODO: Add Caves
            int trees = new Random().Next(0, 10);
            int[,] heights = new int[Chunk.Width, Chunk.Depth];
            int[,] treeBasePositions = new int[trees, 2];

            for (int t = 0; t < trees; t++)
            {
                treeBasePositions[t, 0] = new Random().Next(1, Chunk.Width);
                treeBasePositions[t, 1] = new Random().Next(1, Chunk.Depth);
            }

            //Make a new Chunk
            var chunk = new Chunk(position);
            //Loop through all the blocks in the chunk
            for (int x = 0; x < Chunk.Width; x++)
            {
                for (int z = 0; z < Chunk.Depth; z++)
                {
                    int height = GetHeight(chunk.X * Chunk.Width + x, chunk.Z * Chunk.Depth + z);
                    for (int y = 0; y < height; y++)
                    {
                        if (y == 0) // if at the bottom then set block to bedrock
                            chunk.SetBlockID(new Coordinates3D(x, y, z), BedrockBlock.BlockID);
                        else if (y < height - 1) // if not at the top set the block to dirt or stone depending on height
                        {
                            if (y >= (height / 4) * 3)
                                chunk.SetBlockID(new Coordinates3D(x, y, z), DirtBlock.BlockID);
                            else
                                chunk.SetBlockID(new Coordinates3D(x, y, z), StoneBlock.BlockID);
                        }
                        else if (y < waterLevel) // if below the water set to sand or clay
                        {
                            if (new Random().Next(1, 40) < 5 && y < waterLevel - 4)
                                chunk.SetBlockID(new Coordinates3D(x, y, z), ClayBlock.BlockID);
                            else
                                chunk.SetBlockID(new Coordinates3D(x, y, z), SandBlock.BlockID);
                        }
                        else
                        {
                            // otherwise set the block to grass or gravel rarely
                            chunk.SetBlockID(new Coordinates3D(x, y, z), GrassBlock.BlockID);
                        }
                        // TODO: Biomes
                        //chunk.SetBiome((byte)x, (byte)z, Biome.ExtremeHills);
                        //if (y < waterLevel + 17)
                        //    chunk.SetBiome((byte)x, (byte)z, Biome.ExtremeHillsEdge);
                        //if (y < waterLevel + 10)
                        //    chunk.SetBiome((byte)x, (byte)z, Biome.Beach);
                    }
                    heights[x, z] = height;

                    //create beaches and place water
                    if (height <= waterLevel)
                    {
                        for (int w = 0; w < waterLevel - 3; w++)
                        {
                            if (chunk.GetBlockID(new Coordinates3D(x, w, z)) == 0)
                            {
                                chunk.SetBlockID(new Coordinates3D(x, w, z), StationaryWaterBlock.BlockID);
                            }
                        }
                    }

                    // Generate trees
                    for (int pos = 0; pos < trees; pos++)
                    {
                        int random = new Random().Next(3, 4);
                        int treeBase = heights[treeBasePositions[pos, 0], treeBasePositions[pos, 1]];//chunk.GetHeight((byte)treeBasePositions[pos, 0], (byte)treeBasePositions[pos, 1]);
                        if (treeBasePositions[pos, 0] < 14 && treeBasePositions[pos, 0] > 4 && treeBasePositions[pos, 1] < 14 && treeBasePositions[pos, 1] > 4)
                        {
                            if (treeBase < waterLevel + 10)
                                break;
                            int leafwidth = 4;
                            for (int layer = 0; layer <= height; layer++)
                            {
                                for (int w = 0; w <= leafwidth; w++)
                                {
                                    for (int l = 0; l <= leafwidth; l++)
                                    {
                                        chunk.SetBlockID(new Coordinates3D(treeBasePositions[pos, 0] - (leafwidth / 2) + w, treeBase + layer + random, treeBasePositions[pos, 1] - (leafwidth / 2) + l), LeavesBlock.BlockID);
                                    }
                                }
                                leafwidth -= 1;
                            }

                            for (int t = 0; t <= (random + 2); t++)
                            {
                                chunk.SetBlockID(new Coordinates3D(treeBasePositions[pos, 0], treeBase + t, treeBasePositions[pos, 1]), WoodBlock.BlockID);
                            }
                        }
                    }
                }
            }
            return chunk;
        }

        private int GetHeight(int x, int z)
        {
            var height = -1 * (int)noise.Get2D(x, z);
            if (height <= 0)
                height = height * -1 + 4;
            return height + 40;
        }

        private int RandRange(Random r, int rMin, int rMax)
        {
            return rMin + r.Next() * (rMax - rMin);
        }

        private double RandRange(Random r, double rMin, double rMax)
        {
            return rMin + r.NextDouble() * (rMax - rMin);
        }

        private float RandRange(Random r, float rMin, float rMax)
        {
            return rMin + (float)r.NextDouble() * (rMax - rMin);
        }
    }
}

