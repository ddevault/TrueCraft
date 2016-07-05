using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.Core.World;
using TrueCraft.API.Entities;
using TrueCraft.API.World;
using TrueCraft.API;
using TrueCraft.API.Physics;

namespace TrueCraft.Core.Physics
{
    public class PhysicsEngine : IPhysicsEngine
    {
        public PhysicsEngine(IWorld world, IBlockPhysicsProvider physicsProvider)
        {
            World = world;
            Entities = new List<IPhysicsEntity>();
            EntityLock = new object();
            BlockPhysicsProvider = physicsProvider;
        }

        public IWorld World { get; set; }
        public IBlockPhysicsProvider BlockPhysicsProvider { get; set; }
        public List<IPhysicsEntity> Entities { get; set; }
        private object EntityLock { get; set; }

        public void AddEntity(IPhysicsEntity entity)
        {
            if (Entities.Contains(entity))
                return;
            lock (EntityLock)
                Entities.Add(entity);
        }

        public void RemoveEntity(IPhysicsEntity entity)
        {
            if (!Entities.Contains(entity))
                return;
            lock (EntityLock)
                Entities.Remove(entity);
        }

        public void Update(TimeSpan time)
        {
            double multiplier = time.TotalSeconds;
            if (multiplier == 0)
                return;
            lock (EntityLock)
            {
                for (int i = 0; i < Entities.Count; i++)
                {
                    var entity = Entities[i];
                    if (entity.BeginUpdate())
                    {
                        entity.Velocity -= new Vector3(0, entity.AccelerationDueToGravity * multiplier, 0);
                        entity.Velocity *= 1 - entity.Drag * multiplier;
                        if (entity.Velocity.Distance < 0.001)
                            entity.Velocity = Vector3.Zero;
                        entity.Velocity.Clamp(entity.TerminalVelocity);

                        Vector3 collision, before = entity.Velocity;

                        var aabbEntity = entity as IAABBEntity;
                        if (aabbEntity != null)
                        {
                            if (TestTerrainCollisionY(aabbEntity, out collision))
                                aabbEntity.TerrainCollision(collision, before.Y < 0 ? Vector3.Down : Vector3.Up);
                            if (TestTerrainCollisionX(aabbEntity, out collision))
                                aabbEntity.TerrainCollision(collision, before.X < 0 ? Vector3.Left : Vector3.Right);
                            if (TestTerrainCollisionZ(aabbEntity, out collision))
                                aabbEntity.TerrainCollision(collision, before.Z < 0 ? Vector3.Backwards : Vector3.Forwards);

                            if (TestTerrainCollisionCylinder(aabbEntity, out collision))
                                aabbEntity.TerrainCollision(collision, before);
                        }

                        entity.EndUpdate(entity.Position + entity.Velocity);
                    }
                }
            }
        }

        private BoundingBox GetAABBVelocityBox(IAABBEntity entity)
        {
            var min = new Vector3(
                Math.Min(entity.BoundingBox.Min.X, entity.BoundingBox.Min.X + entity.Velocity.X),
                Math.Min(entity.BoundingBox.Min.Y, entity.BoundingBox.Min.Y + entity.Velocity.Y),
                Math.Min(entity.BoundingBox.Min.Z, entity.BoundingBox.Min.Z + entity.Velocity.Z)
            );
            var max = new Vector3(
                Math.Max(entity.BoundingBox.Max.X, entity.BoundingBox.Max.X + entity.Velocity.X),
                Math.Max(entity.BoundingBox.Max.Y, entity.BoundingBox.Max.Y + entity.Velocity.Y),
                Math.Max(entity.BoundingBox.Max.Z, entity.BoundingBox.Max.Z + entity.Velocity.Z)
            );
            return new BoundingBox(min, max);
        }

        private void AdjustVelocityForCollision(IAABBEntity entity, BoundingBox problem)
        {
            var velocity = entity.Velocity;
            if (entity.Velocity.X < 0)
                velocity.X = entity.BoundingBox.Min.X - problem.Max.X;
            if (entity.Velocity.X > 0)
                velocity.X = entity.BoundingBox.Max.X - problem.Min.X;
            if (entity.Velocity.Y < 0)
                velocity.Y = entity.BoundingBox.Min.Y - problem.Max.Y;
            if (entity.Velocity.Y > 0)
                velocity.Y = entity.BoundingBox.Max.Y - problem.Min.Y;
            if (entity.Velocity.Z < 0)
                velocity.Z = entity.BoundingBox.Min.Z - problem.Max.Z;
            if (entity.Velocity.Z > 0)
                velocity.Z = entity.BoundingBox.Max.Z - problem.Min.Z;
            entity.Velocity = velocity;
        }

        public bool TestTerrainCollisionCylinder(IAABBEntity entity, out Vector3 collisionPoint)
        {
            collisionPoint = Vector3.Zero;
            var testBox = GetAABBVelocityBox(entity);
            var testCylinder = new BoundingCylinder(testBox.Min, testBox.Max,
                entity.BoundingBox.Min.DistanceTo(entity.BoundingBox.Max));

            bool collision = false;
            for (int x = (int)(Math.Floor(testBox.Min.X)); x <= (int)(Math.Ceiling(testBox.Max.X)); x++)
            {
                for (int z = (int)(Math.Floor(testBox.Min.Z)); z <= (int)(Math.Ceiling(testBox.Max.Z)); z++)
                {
                    for (int y = (int)(Math.Floor(testBox.Min.Y)); y <= (int)(Math.Ceiling(testBox.Max.Y)); y++)
                    {
                        var coords = new Coordinates3D(x, y, z);
                        if (!World.IsValidPosition(coords))
                            continue;

                        var _box = BlockPhysicsProvider.GetBoundingBox(World, coords);
                        if (_box == null)
                            continue;

                        var box = _box.Value.OffsetBy(coords);
                        if (testCylinder.Intersects(box))
                        {
                            if (testBox.Intersects(box))
                            {
                                collision = true;
                                AdjustVelocityForCollision(entity, box);
                                testBox = GetAABBVelocityBox(entity);
                                testCylinder = new BoundingCylinder(testBox.Min, testBox.Max,
                                    entity.BoundingBox.Min.DistanceTo(entity.BoundingBox.Max));
                                collisionPoint = coords;
                            }
                        }
                    }
                }
            }
            return collision;
        }

        public bool TestTerrainCollisionY(IAABBEntity entity, out Vector3 collisionPoint)
        {
            // Things we need to do:
            // 1 - expand bounding box to include the destination and everything within
            // 2 - collect all blocks within that area
            // 3 - test bounding boxes in direction of motion

            collisionPoint = Vector3.Zero;

            if (entity.Velocity.Y == 0)
                return false;

            bool negative;

            BoundingBox testBox;
            if (entity.Velocity.Y < 0)
            {
                testBox = new BoundingBox(
                    new Vector3(entity.BoundingBox.Min.X,
                        entity.BoundingBox.Min.Y + entity.Velocity.Y,
                        entity.BoundingBox.Min.Z),
                    entity.BoundingBox.Max);
                negative = true;
            }
            else
            {
                testBox = new BoundingBox(
                    entity.BoundingBox.Min,
                    new Vector3(entity.BoundingBox.Max.X,
                        entity.BoundingBox.Max.Y + entity.Velocity.Y,
                        entity.BoundingBox.Max.Z));
                negative = false;
            }

            double? collisionExtent = null;
            for (int x = (int)(Math.Floor(testBox.Min.X)); x <= (int)(Math.Ceiling(testBox.Max.X)); x++)
            {
                for (int z = (int)(Math.Floor(testBox.Min.Z)); z <= (int)(Math.Ceiling(testBox.Max.Z)); z++)
                {
                    for (int y = (int)(Math.Floor(testBox.Min.Y)); y <= (int)(Math.Ceiling(testBox.Max.Y)); y++)
                    {
                        var coords = new Coordinates3D(x, y, z);
                        if (!World.IsValidPosition(coords))
                            continue;

                        var _box = BlockPhysicsProvider.GetBoundingBox(World, coords);
                        if (_box == null)
                            continue;

                        var box = _box.Value.OffsetBy(coords);
                        if (testBox.Intersects(box))
                        {
                            if (negative)
                            {
                                if (collisionExtent == null || collisionExtent.Value < box.Max.Y)
                                {
                                    collisionExtent = box.Max.Y;
                                    collisionPoint = coords;
                                }
                            }
                            else
                            {
                                if (collisionExtent == null || collisionExtent.Value > box.Min.Y)
                                {
                                    collisionExtent = box.Min.Y;
                                    collisionPoint = coords;
                                }
                            }
                        }
                    }
                }
            }

            if (collisionExtent != null) // Collision detected, adjust accordingly
            {
                var extent = collisionExtent.Value;
                double diff;
                if (negative)
                    diff = -(entity.BoundingBox.Min.Y - extent);
                else
                    diff = extent - entity.BoundingBox.Max.Y;
                entity.Velocity = new Vector3(entity.Velocity.X, diff, entity.Velocity.Z);
                return true;
            }
            return false;
        }

        public bool TestTerrainCollisionX(IAABBEntity entity, out Vector3 collisionPoint)
        {
            // Things we need to do:
            // 1 - expand bounding box to include the destination and everything within
            // 2 - collect all blocks within that area
            // 3 - test bounding boxes in direction of motion

            collisionPoint = Vector3.Zero;

            if (entity.Velocity.X == 0)
                return false;

            bool negative;

            BoundingBox testBox;
            if (entity.Velocity.X < 0)
            {
                testBox = new BoundingBox(
                    new Vector3(
                        entity.BoundingBox.Min.X + entity.Velocity.X,
                        entity.BoundingBox.Min.Y,
                        entity.BoundingBox.Min.Z),
                    entity.BoundingBox.Max);
                negative = true;
            }
            else
            {
                testBox = new BoundingBox(
                    entity.BoundingBox.Min,
                    new Vector3(
                        entity.BoundingBox.Max.X + entity.Velocity.X,
                        entity.BoundingBox.Max.Y,
                        entity.BoundingBox.Max.Z));
                negative = false;
            }

            double? collisionExtent = null;
            for (int x = (int)(Math.Floor(testBox.Min.X)); x <= (int)(Math.Ceiling(testBox.Max.X)); x++)
            {
                for (int z = (int)(Math.Floor(testBox.Min.Z)); z <= (int)(Math.Ceiling(testBox.Max.Z)); z++)
                {
                    for (int y = (int)(Math.Floor(testBox.Min.Y)); y <= (int)(Math.Ceiling(testBox.Max.Y)); y++)
                    {
                        var coords = new Coordinates3D(x, y, z);
                        if (!World.IsValidPosition(coords))
                            continue;

                        var _box = BlockPhysicsProvider.GetBoundingBox(World, coords);
                        if (_box == null)
                            continue;

                        var box = _box.Value.OffsetBy(coords);
                        if (testBox.Intersects(box))
                        {
                            if (negative)
                            {
                                if (collisionExtent == null || collisionExtent.Value < box.Max.X)
                                {
                                    collisionExtent = box.Max.X;
                                    collisionPoint = coords;
                                }
                            }
                            else
                            {
                                if (collisionExtent == null || collisionExtent.Value > box.Min.X)
                                {
                                    collisionExtent = box.Min.X;
                                    collisionPoint = coords;
                                }
                            }
                        }
                    }
                }
            }

            if (collisionExtent != null) // Collision detected, adjust accordingly
            {
                var extent = collisionExtent.Value;
                double diff;
                if (negative)
                    diff = -(entity.BoundingBox.Min.X - extent);
                else
                    diff = extent - entity.BoundingBox.Max.X;
                entity.Velocity = new Vector3(diff, entity.Velocity.Y, entity.Velocity.Z);
                return true;
            }
            return false;
        }

        public bool TestTerrainCollisionZ(IAABBEntity entity, out Vector3 collisionPoint)
        {
            // Things we need to do:
            // 1 - expand bounding box to include the destination and everything within
            // 2 - collect all blocks within that area
            // 3 - test bounding boxes in direction of motion

            collisionPoint = Vector3.Zero;

            if (entity.Velocity.Z == 0)
                return false;

            bool negative;

            BoundingBox testBox;
            if (entity.Velocity.Z < 0)
            {
                testBox = new BoundingBox(
                    new Vector3(
                        entity.BoundingBox.Min.X,
                        entity.BoundingBox.Min.Y,
                        entity.BoundingBox.Min.Z + entity.Velocity.Z),
                    entity.BoundingBox.Max);
                negative = true;
            }
            else
            {
                testBox = new BoundingBox(
                    entity.BoundingBox.Min,
                    new Vector3(
                        entity.BoundingBox.Max.X,
                        entity.BoundingBox.Max.Y,
                        entity.BoundingBox.Max.Z + entity.Velocity.Z));
                negative = false;
            }

            double? collisionExtent = null;
            for (int x = (int)(Math.Floor(testBox.Min.X)); x <= (int)(Math.Ceiling(testBox.Max.X)); x++)
            {
                for (int z = (int)(Math.Floor(testBox.Min.Z)); z <= (int)(Math.Ceiling(testBox.Max.Z)); z++)
                {
                    for (int y = (int)(Math.Floor(testBox.Min.Y)); y <= (int)(Math.Ceiling(testBox.Max.Y)); y++)
                    {
                        var coords = new Coordinates3D(x, y, z);
                        if (!World.IsValidPosition(coords))
                            continue;

                        var _box = BlockPhysicsProvider.GetBoundingBox(World, coords);
                        if (_box == null)
                            continue;

                        var box = _box.Value.OffsetBy(coords);
                        if (testBox.Intersects(box))
                        {
                            if (negative)
                            {
                                if (collisionExtent == null || collisionExtent.Value < box.Max.Z)
                                {
                                    collisionExtent = box.Max.Z;
                                    collisionPoint = coords;
                                }
                            }
                            else
                            {
                                if (collisionExtent == null || collisionExtent.Value > box.Min.Z)
                                {
                                    collisionExtent = box.Min.Z;
                                    collisionPoint = coords;
                                }
                            }
                        }
                    }
                }
            }

            if (collisionExtent != null) // Collision detected, adjust accordingly
            {
                var extent = collisionExtent.Value;
                double diff;
                if (negative)
                    diff = -(entity.BoundingBox.Min.Z - extent);
                else
                    diff = extent - entity.BoundingBox.Max.Z;
                entity.Velocity = new Vector3(entity.Velocity.X, entity.Velocity.Y, diff);
                return true;
            }
            return false;
        }
    }
}
