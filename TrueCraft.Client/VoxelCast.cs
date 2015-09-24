using System;
using TrueCraft.API;
using TrueCraft.Core.Logic;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Client
{
    /// <summary>
    /// Efficient ray caster that can cast a ray into a voxel map
    /// and return the voxel that it intersects with.
    /// </summary>
    public static class VoxelCast
    {
        // Thanks to http://gamedev.stackexchange.com/questions/47362/cast-ray-to-select-block-in-voxel-game

        public static Tuple<Coordinates3D, BlockFace> Cast(ReadOnlyWorld world,
            Ray ray, BlockRepository repository, double max)
        {
            var origin = ray.Position.Floor();
            var direction = ray.Direction;
            var step = new Vector3(SigNum(ray.Position.X), SigNum(ray.Position.Y), SigNum(ray.Position.Z));
            var tMax = new Vector3(
                IntBound(origin.X, direction.X),
                IntBound(origin.Y, direction.Y),
                IntBound(origin.Z, direction.Z));
            var tDelta = new Vector3(
                step.X / direction.X,
                step.Y / direction.Y,
                step.Z / direction.Z);
            BlockFace face = BlockFace.PositiveY;

            if (ray.Direction == Vector3.Zero)
                return null;

            max /= Math.Sqrt(ray.Direction.X * ray.Direction.X
                + ray.Direction.Y * ray.Direction.Y
                + ray.Direction.Z * ray.Direction.Z);

            while (world.IsValidPosition((Coordinates3D)origin))
            {
                var provider = repository.GetBlockProvider(world.GetBlockID((Coordinates3D)origin));
                var _box = provider.BoundingBox;
                if (_box != null)
                {
                    var box = _box.Value.OffsetBy((Coordinates3D)origin);
                    if (ray.Intersects(box) != null)
                        return new Tuple<Coordinates3D, BlockFace>((Coordinates3D)origin, face);
                }

                if (tMax.X < tMax.Y)
                {
                    if (tMax.X < tMax.Z)
                    {
                        if (tMax.X > max)
                            return null;
                        // Update which cube we are now in.
                        origin.X += step.X;
                        // Adjust tMaxX to the next X-oriented boundary crossing.
                        tMax.X += tDelta.X;
                        // Record the normal vector of the cube face we entered.
                        if (step.X < 0)
                            face = BlockFace.PositiveX;
                        else
                            face = BlockFace.NegativeX;
                    }
                    else
                    {
                        if (tMax.Z > max)
                            return null;
                        origin.Z += step.Z;
                        tMax.Z += tDelta.Z;
                        if (step.Z < 0)
                            face = BlockFace.PositiveZ;
                        else
                            face = BlockFace.NegativeZ;
                    }
                }
                else
                {
                    if (tMax.Y < tMax.Z)
                    {
                        if (tMax.Y > max)
                            return null;
                        origin.Y += step.Y;
                        tMax.Y += tDelta.Y;
                        if (step.Y < 0)
                            face = BlockFace.PositiveY;
                        else
                            face = BlockFace.NegativeY;
                    }
                    else
                    {
                        // Identical to the second case, repeated for simplicity in
                        // the conditionals.
                        if (tMax.Z > max)
                            break;
                        origin.Z += step.Z;
                        tMax.Z += tDelta.Z;
                        if (step.Z < 0)
                            face = BlockFace.PositiveZ;
                        else
                            face = BlockFace.NegativeZ;
                    }
                }
            }

            return null;
        }

        private static double IntBound(double s, double ds)
        {
            // Find the smallest positive t such that s+t*ds is an integer.
            if (ds < 0)
            {
                return IntBound(-s, -ds);
            }
            else
            {
                s = Mod(s, 1);
                // problem is now s+t*ds = 1
                return (1 - s) / ds;
            }
        }

        private static int SigNum(double x)
        {
            return x > 0 ? 1 : x < 0 ? -1 : 0;
        }

        private static double Mod(double value, double modulus)
        {
            return (value % modulus + modulus) % modulus;
        }
    }
}
