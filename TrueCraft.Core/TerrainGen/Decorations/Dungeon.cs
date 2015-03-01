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
        int MaxEntrances = 5;

        public override bool ValidLocation(Coordinates3D location)
        {
            var OffsetSize = Size + new Vector3(1, 1, 1);
            if (location.X + (int)OffsetSize.X >= Chunk.Width || location.Z + (int)OffsetSize.Z >= Chunk.Depth || location.Y + (int)OffsetSize.Y >= Chunk.Height)
                return false;
            return true;
        }

        public override bool GenerateAt(IWorld world, IChunk chunk, Coordinates3D location)
        {
            if (!ValidLocation(location))
                return false;
            Random R = new Random(world.Seed);

            //Generate room
            GenerateCuboid(chunk, location, Size, CobblestoneBlock.BlockID, 0x0, 0x2);

            //Randomly add mossy cobblestone to floor
            MossFloor(chunk, location, R);

            //Place Spawner
            chunk.SetBlockID(new Coordinates3D((int)(location.X + ((Size.X + 1) / 2)), (int)((location + Coordinates3D.Up).Y), (int)(location.Z + ((Size.Z + 1) / 2))), MonsterSpawnerBlock.BlockID);
            
            //Create entrances
            CreateEntraces(chunk, location, R);

            //Place Chests
            PlaceChests(chunk, location, R);
            return true;
        }

        private void CreateEntraces(IChunk chunk, Coordinates3D location, Random R)
        {
            int Entrances = 0;
            var Above = location + Coordinates3D.Up;
            for (int X = location.X; X < location.X + Size.X; X++)
            {
                if (Entrances >= MaxEntrances)
                    break;
                for (int Z = location.Z; Z < location.Z + Size.Z; Z++)
                {
                    if (Entrances >= MaxEntrances)
                        break;
                    if (R.Next(0, 3) == 0 && IsCuboidWall(new Coordinates2D(X, Z), location, Size) && !IsCuboidCorner(new Coordinates2D(X, Z), location, Size))
                    {
                        var BlockLocation = new Coordinates3D(X, Above.Y, Z);
                        chunk.SetBlockID(BlockLocation, AirBlock.BlockID);
                        chunk.SetBlockID(BlockLocation + Coordinates3D.Up, AirBlock.BlockID);
                    }
                }
            }
        }

        private void MossFloor(IChunk chunk, Coordinates3D location, Random R)
        {
            for (int X = location.X; X < location.X + Size.X; X++)
            {
                for (int Z = location.Z; Z < location.Z + Size.Z; Z++)
                {
                    if (R.Next(0, 3) == 0)
                        chunk.SetBlockID(new Coordinates3D(X, location.Y, Z), MossStoneBlock.BlockID);
                }
            }
        }

        private void PlaceChests(IChunk chunk, Coordinates3D location, Random R)
        {
            var Above = location + Coordinates3D.Up;
            var chests = R.Next(0, 2);
            for (int I = 0; I < chests; I++)
            {
                for (int Attempts = 0; Attempts < 10; Attempts++)
                {
                    var X = R.Next(location.X, location.X + (int)Size.X);
                    var Z = R.Next(location.Z, location.Z + (int)Size.Z);
                    if (!IsCuboidWall(new Coordinates2D(X, Z), location, Size) && !IsCuboidCorner(new Coordinates2D(X, Z), location, Size))
                    {
                        if (NeighboursBlock(chunk, new Coordinates3D(X, Above.Y, Z), CobblestoneBlock.BlockID))
                        {
                            chunk.SetBlockID(new Coordinates3D(X, Above.Y, Z), ChestBlock.BlockID);
                            break;
                        }
                    }
                }
            }
        }
    }
}