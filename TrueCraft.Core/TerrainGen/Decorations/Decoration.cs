using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.World;
using TrueCraft.API;
using TrueCraft.Core.World;

namespace TrueCraft.Core.TerrainGen.Decorations
{
    public abstract class Decoration : IDecoration
    {
        public virtual bool ValidLocation(Coordinates3D location) { return true; }

        public abstract bool GenerateAt(IWorld world, IChunk chunk, Coordinates3D location);

        public static bool IsCuboidWall(Coordinates2D location, Coordinates3D start, Vector3 size)
        {
            return location.X.Equals(start.X) || location.Z.Equals(start.Z) || location.X.Equals(start.X + (int)size.X - 1) || location.Z.Equals(start.Z + (int)size.Z - 1);
        }

        public static bool IsCuboidCorner(Coordinates2D location, Coordinates3D start, Vector3 size)
        {
            return location.X.Equals(start.X) && location.Z.Equals(start.Z) || location.X.Equals(start.X) && location.Z.Equals(start.Z + (int)size.Z - 1) || location.X.Equals(start.X + (int)size.X - 1) && location.Z.Equals(start.Z) || location.X.Equals(start.X + (int)size.X - 1) && location.Z.Equals(start.Z + (int)size.Z - 1);
        }

        public static bool NeighboursBlock(IChunk chunk, Coordinates3D location, byte block, byte meta = 0x0)
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
                if (chunk.GetBlockID(Check).Equals(block))
                {
                    if (meta != 0x0 && chunk.GetMetadata(Check) != meta)
                        return false;
                    return true;
                }
            }
            return false;
        }

        public static void GenerateColumn(IChunk chunk, Coordinates3D location, int height, byte block, byte meta = 0x0)
        {
            for (int offset = 0; offset < height; offset++)
            {
                Coordinates3D blockLocation = location + new Coordinates3D(0, offset, 0);
                if (blockLocation.Y >= Chunk.Height)
                    return;
                chunk.SetBlockID(blockLocation, block);
                chunk.SetMetadata(blockLocation, meta);
            }
        }

        /*
         * Cuboid Modes
         * 0x0 - Solid cuboid of the specified block
         * 0x1 - Hollow cuboid of the specified block
         * 0x2 - Outlines the area of the cuboid using the specified block
         */
        public static void GenerateCuboid(IChunk chunk, Coordinates3D location, Vector3 size, byte block, byte meta = 0x0, byte mode = 0x0)
        {
            //If mode is 0x2 offset the size by 2 and change mode to 0x1
            if (mode.Equals(0x2))
            {
                size += new Vector3(2, 2, 2);
                mode = 0x1;
            }

            for (int W = location.X; W < location.X + size.X; W++)
            {
                for (int L = location.Z; L < location.Z + size.Z; L++)
                {
                    for (int H = location.Y; H < location.Y + size.Y; H++)
                    {

                        if (W < 0 || W >= Chunk.Width || L < 0 || L >= Chunk.Depth || H < 0 || H >= Chunk.Height)
                            continue;
                        Coordinates3D BlockLocation = new Coordinates3D(W, H, L);
                        if (!H.Equals(location.Y) && !H.Equals(location.Y + (int)size.Y - 1) && !IsCuboidWall(new Coordinates2D(W, L), location, size) && !IsCuboidCorner(new Coordinates2D(W, L), location, size))
                            continue;

                        chunk.SetBlockID(BlockLocation, block);
                        if (meta != 0x0)
                            chunk.SetMetadata(BlockLocation, meta);
                    }
                }
            }
        }

        protected void GenerateVanillaLeaves(IChunk chunk, Coordinates3D location, int radius, byte block, byte meta = 0x0)
        {
            int RadiusOffset = radius;
            for (int YOffset = -radius; YOffset <= radius; YOffset = (YOffset + 1))
            {
                int Y = location.Y + YOffset;
                if (Y > Chunk.Height)
                    continue;
                GenerateVanillaCircle(chunk, new Coordinates3D(location.X, Y, location.Z), RadiusOffset, block, meta);
                if (YOffset != -radius && YOffset % 2 == 0)
                    RadiusOffset--;
            }
        }

        protected void GenerateVanillaCircle(IChunk chunk, Coordinates3D location, int radius, byte block, byte meta = 0x0, double corner = 0)
        {
            for (int I = -radius; I <= radius; I = (I + 1))
            {
                for (int J = -radius; J <= radius; J = (J + 1))
                {
                    int Max = (int)Math.Sqrt((I * I) + (J * J));
                    if (Max <= radius)
                    {
                        if (I.Equals(-radius) && J.Equals(-radius) || I.Equals(-radius) && J.Equals(radius) || I.Equals(radius) && J.Equals(-radius) || I.Equals(radius) && J.Equals(radius))
                        {
                            if (corner + radius * 0.2 < 0.4 || corner + radius * 0.2 > 0.7 || corner.Equals(0))
                                continue;
                        }
                        int X = location.X + I;
                        int Z = location.Z + J;
                        Coordinates3D CurrentBlock = new Coordinates3D(X, location.Y, Z);
                        if (chunk.GetBlockID(CurrentBlock).Equals(0))
                        {
                            chunk.SetBlockID(CurrentBlock, block);
                            chunk.SetMetadata(CurrentBlock, meta);
                        }
                    }
                }
            }
        }

        protected void GenerateCircle(IChunk chunk, Coordinates3D location, int radius, byte block, byte meta = 0x0)
        {
            for (int I = -radius; I <= radius; I = (I + 1))
            {
                for (int J = -radius; J <= radius; J = (J + 1))
                {
                    int Max = (int)Math.Sqrt((I * I) + (J * J));
                    if (Max <= radius)
                    {
                        int X = location.X + I;
                        int Z = location.Z + J;

                        if (X < 0 || X >= Chunk.Width || Z < 0 || Z >= Chunk.Depth)
                            continue;

                        Coordinates3D CurrentBlock = new Coordinates3D(X, location.Y, Z);
                        if (chunk.GetBlockID(CurrentBlock).Equals(0))
                        {
                            chunk.SetBlockID(CurrentBlock, block);
                            chunk.SetMetadata(CurrentBlock, meta);
                        }
                    }
                }
            }
        }

        protected static void GenerateSphere(IChunk chunk, Coordinates3D location, int radius, byte block, byte meta = 0x0)
        {
            for (int I = -radius; I <= radius; I = (I + 1))
            {
                for (int J = -radius; J <= radius; J = (J + 1))
                {
                    for (int K = -radius; K <= radius; K = (K + 1))
                    {
                        int Max = (int)Math.Sqrt((I * I) + (J * J) + (K * K));
                        if (Max <= radius)
                        {
                            int X = location.X + I;
                            int Y = location.Y + K;
                            int Z = location.Z + J;

                            if (X < 0 || X >= Chunk.Width || Z < 0 || Z >= Chunk.Depth || Y < 0 || Y >= Chunk.Height)
                                continue;

                            Coordinates3D CurrentBlock = new Coordinates3D(X, Y, Z);
                            if (chunk.GetBlockID(CurrentBlock).Equals(0))
                            {
                                chunk.SetBlockID(CurrentBlock, block);
                                chunk.SetMetadata(CurrentBlock, meta);
                            }
                        }
                    }
                }
            }
        }
    }
}