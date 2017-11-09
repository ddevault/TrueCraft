using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API;
using TrueCraft.API.World;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.Core.World;
using TrueCraft.Core.TerrainGen.Noise;

namespace TrueCraft.Core.TerrainGen.Decorations
{
    public class Dungeon : Decoration
    {
        Vector3 Size = new Vector3(7, 5, 7);

        const int MaxEntrances = 5;

        public override bool ValidLocation(Coordinates3D location)
        {
            var OffsetSize = Size + new Vector3(1, 1, 1);
            if (location.X + (int)OffsetSize.X >= Chunk.Width
                || location.Z + (int)OffsetSize.Z >= Chunk.Depth
                || location.Y + (int)OffsetSize.Y >= Chunk.Height)
                return false;
            return true;
        }

        public override bool GenerateAt(IWorldSeed world, IChunk chunk, Coordinates3D location)
        {
            Console.WriteLine("Dungeon in chunk {0}", chunk.Coordinates);
            if (!ValidLocation(location))
                return false;

            var random = new Random(world.Seed);

            //Generate room
            GenerateCuboid(chunk, location, Size, CobblestoneBlock.BlockID, 0x0, 0x2);

            //Randomly add mossy cobblestone to floor
            MossFloor(chunk, location, random);

            //Place Spawner
            chunk.SetBlockID(new Coordinates3D((int)(location.X + ((Size.X + 1) / 2)), (int)((location + Coordinates3D.Up).Y), (int)(location.Z + ((Size.Z + 1) / 2))), MonsterSpawnerBlock.BlockID);
            
            //Create entrances
            CreateEntraces(chunk, location, random);

            //Place Chests
            PlaceChests(chunk, location, random);

            return true;
        }

        private void CreateEntraces(IChunk chunk, Coordinates3D location, Random random)
        {
            int entrances = 0;
            var above = location + Coordinates3D.Up;
            for (int X = location.X; X < location.X + Size.X; X++)
            {
                if (entrances >= MaxEntrances)
                    break;
                for (int Z = location.Z; Z < location.Z + Size.Z; Z++)
                {
                    if (entrances >= MaxEntrances)
                        break;
                    if (random.Next(0, 3) == 0 && IsCuboidWall(new Coordinates2D(X, Z), location, Size)
                        && !IsCuboidCorner(new Coordinates2D(X, Z), location, Size))
                    {
                        var blockLocation = new Coordinates3D(X, above.Y, Z);
                        if (blockLocation.X < 0 || blockLocation.X >= Chunk.Width
                            || blockLocation.Z < 0 || blockLocation.Z >= Chunk.Depth
                            || blockLocation.Y < 0 || blockLocation.Y >= Chunk.Height)
                            continue;
                        chunk.SetBlockID(blockLocation, AirBlock.BlockID);
                        chunk.SetBlockID(blockLocation + Coordinates3D.Up, AirBlock.BlockID);
                        entrances++;
                    }
                }
            }
        }

        private void MossFloor(IChunk chunk, Coordinates3D location, Random random)
        {
            for (int x = location.X; x < location.X + Size.X; x++)
            {
                for (int z = location.Z; z < location.Z + Size.Z; z++)
                {
                    if (x < 0 || x >= Chunk.Width
                            || z < 0 || z >= Chunk.Depth
                            || location.Y < 0 || location.Y >= Chunk.Height)
                            continue;
                    if (random.Next(0, 3) == 0)
                        chunk.SetBlockID(new Coordinates3D(x, location.Y, z), MossStoneBlock.BlockID);
                }
            }
        }

        private void PlaceChests(IChunk chunk, Coordinates3D location, Random random)
        {
            var above = location + Coordinates3D.Up;
            var chests = random.Next(0, 2);
            for (int i = 0; i < chests; i++)
            {
                for (int attempts = 0; attempts < 10; attempts++)
                {
                    var x = random.Next(location.X, location.X + (int)Size.X);
                    var z = random.Next(location.Z, location.Z + (int)Size.Z);
                    if (!IsCuboidWall(new Coordinates2D(x, z), location, Size) && !IsCuboidCorner(new Coordinates2D(x, z), location, Size))
                    {
                        if (NeighboursBlock(chunk, new Coordinates3D(x, above.Y, z), CobblestoneBlock.BlockID))
                        {
                            if (x < 0 || x >= Chunk.Width
                                || z < 0 || z >= Chunk.Depth
                                || above.Y < 0 || above.Y >= Chunk.Height)
                            continue;
                            chunk.SetBlockID(new Coordinates3D(x, above.Y, z), ChestBlock.BlockID);
                            break;
                        }
                    }
                }
            }
        }
    }
}