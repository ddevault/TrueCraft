using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API;
using TrueCraft.Core.World;

namespace TrueCraft.Core
{
    public static class MathHelper
    {
        /// <summary>
        /// A global <see cref="System.Random"/> instance.
        /// </summary>
        public static Random Random = new Random();

        /// <summary>
        /// Maps a float from 0...360 to 0...255
        /// </summary>
        /// <param name="value"></param>
        public static sbyte CreateRotationByte(float value)
        {
            return (sbyte)(((value % 360) / 360) * 256);
        }

        public static float UnpackRotationByte(sbyte value)
        {
            return (value / 256f) * 360f;
        }

        public static int CreateAbsoluteInt(double value)
        {
            return (int)(value * 32);
        }

        public static Coordinates3D BlockFaceToCoordinates(BlockFace face)
        {
            switch (face)
            {
                case BlockFace.NegativeY:
                    return Coordinates3D.Down;
                case BlockFace.PositiveY:
                    return Coordinates3D.Up;
                case BlockFace.NegativeZ:
                    return Coordinates3D.Backwards;
                case BlockFace.PositiveZ:
                    return Coordinates3D.Forwards;
                case BlockFace.NegativeX:
                    return Coordinates3D.Left;
                default:
                    return Coordinates3D.Right;
            }
        }

        public static double Distance2D(double a1, double a2, double b1, double b2)
        {
            return Math.Sqrt(Math.Pow(b1 - a1, 2) + Math.Pow(b2 - a2, 2));
        }

        public static Direction DirectionByRotationFlat(float yaw, bool invert = false)
        {
            byte direction = (byte)((int)Math.Floor((yaw * 4F) / 360F + 0.5D) & 3);
            if (invert)
                switch (direction)
                {
                    case 0: return Direction.North;
                    case 1: return Direction.East;
                    case 2: return Direction.South;
                    case 3: return Direction.West;
                }
            else
                switch (direction)
                {
                    case 0: return Direction.South;
                    case 1: return Direction.West;
                    case 2: return Direction.North;
                    case 3: return Direction.East;
                }
            return 0;
        }

        public static Direction DirectionByRotation(Vector3 source, float yaw, Vector3 position, bool invert = false)
        {
            // TODO: Figure out some algorithm based on player's look yaw
            double d = Math.Asin((source.Y - position.Y) / position.DistanceTo(source));
            if (d > (Math.PI / 4)) return invert ? (Direction)1 : (Direction)0;
            if (d < -(Math.PI / 4)) return invert ? (Direction)0 : (Direction)1;
            return DirectionByRotationFlat(yaw, invert);
        }

        /// <summary>
        /// Gets a byte representing block direction based on the rotation
        /// of the entity that placed it.
        /// </summary>
        public static Vector3 FowardVector(float yaw, bool invert = false)
        {
            Direction value = (Direction)DirectionByRotationFlat(yaw, invert);
            switch (value)
            {
                case Direction.East:
                    return Vector3.East;
                case Direction.West:
                    return Vector3.West;
                case Direction.North:
                    return Vector3.North;
                case Direction.South:
                    return Vector3.South;
                default:
                    return Vector3.Zero;
            }
        }

        public static Vector3 GetVectorTowards(Vector3 a, Vector3 b)
        {
            double angle = Math.Asin((a.X - b.X) / Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Z - b.Z, 2)));
            if (a.Z > b.Z) angle += Math.PI;
            return RotateY(Vector3.Forwards, angle);
        }

        public static Vector3 RotateX(Vector3 vector, double rotation) // TODO: Matrix
        {
            rotation = -rotation; // the algorithms I found were left-handed
            return new Vector3(
                vector.X,
                vector.Y * Math.Cos(rotation) - vector.Z * Math.Sin(rotation),
                vector.Y * Math.Sin(rotation) + vector.Z * Math.Cos(rotation));
        }

        public static Vector3 RotateY(Vector3 vector, double rotation)
        {
            rotation = -rotation; // the algorithms I found were left-handed
            return new Vector3(
                vector.Z * Math.Sin(rotation) + vector.X * Math.Cos(rotation),
                vector.Y,
                vector.Z * Math.Cos(rotation) - vector.X * Math.Sin(rotation));
        }

        public static Vector3 RotateZ(Vector3 vector, double rotation)
        {
            rotation = -rotation; // the algorithms I found were left-handed
            return new Vector3(
                vector.X * Math.Cos(rotation) - vector.Y * Math.Sin(rotation),
                vector.X * Math.Sin(rotation) + vector.Y * Math.Cos(rotation),
                vector.Z);
        }

        public static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        public static double RadiansToDegrees(double radians)
        {
            return radians * (180 / Math.PI);
        }

        public static double ToNotchianYaw(double yaw)
        {
            return RadiansToDegrees(Math.PI - yaw);
        }

        public static double ToNotchianPitch(double pitch)
        {
            return RadiansToDegrees(-pitch);
        }

        public static int ChunkToBlockX(int BlockCoord, int ChunkX)
        {
            return ChunkX * Chunk.Width + BlockCoord;
        }

        public static int ChunkToBlockZ(int BlockCoord, int ChunkZ)
        {
            return ChunkZ * Chunk.Depth + BlockCoord;
        }

        /// <summary>
        /// Returns a value indicating the most extreme value of the
        /// provided Vector.
        /// </summary>
        public static unsafe CollisionPoint GetCollisionPoint(Vector3 velocity)
        {
            // NOTE: Does this really need to be so unsafe?
            int index = 0;
            void* vPtr = &velocity;
            double* ptr = (double*)vPtr;
            double max = 0;
            for (int i = 0; i < 3; i++)
            {
                double value = *(ptr + i);
                if (max < Math.Abs(value))
                {
                    index = i;
                    max = Math.Abs(value);
                }
            }
            switch (index)
            {
                case 0:
                    if (velocity.X < 0)
                        return CollisionPoint.NegativeX;
                    return CollisionPoint.PositiveX;
                case 1:
                    if (velocity.Y < 0)
                        return CollisionPoint.NegativeY;
                    return CollisionPoint.PositiveY;
                default:
                    if (velocity.Z < 0)
                        return CollisionPoint.NegativeZ;
                    return CollisionPoint.PositiveZ;
            }
        }
    }

    public enum Direction
    {
        Bottom = 0,
        Top = 1,
        North = 2,
        South = 3,
        West = 4,
        East = 5
    }

    public enum CollisionPoint
    {
        PositiveX,
        NegativeX,
        PositiveY,
        NegativeY,
        PositiveZ,
        NegativeZ
    }
}
