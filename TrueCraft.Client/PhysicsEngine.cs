using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.Core.World;
using TrueCraft.API.Entities;
using TrueCraft.API.World;
using TrueCraft.API;
using TrueCraft.API.Physics;

namespace TrueCraft.Client
{
    // This is a stripped down version of the physics engine that the server uses
    // The only thing we use it for is our own movement
    public class PhysicsEngine
    {
        public PhysicsEngine(ReadOnlyWorld world, IBlockPhysicsProvider physicsProvider)
        {
            World = world;
            Entities = new List<IPhysicsEntity>();
            EntityLock = new object();
            LastUpdate = DateTime.MinValue;
            BlockPhysicsProvider = physicsProvider;
            MillisecondsBetweenUpdates = 1000 / 20;
        }

        public int MillisecondsBetweenUpdates { get; set; }
        public ReadOnlyWorld World { get; set; }
        public IBlockPhysicsProvider BlockPhysicsProvider { get; set; }
        public List<IPhysicsEntity> Entities { get; set; }
        private object EntityLock { get; set; }
        private DateTime LastUpdate { get; set; }

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

        private BoundingBox TempBoundingBox;
        public void Update()
        {
            double multipler = (DateTime.UtcNow - LastUpdate).TotalMilliseconds / MillisecondsBetweenUpdates;
            if (LastUpdate == DateTime.MinValue)
                multipler = 1;
            if (multipler > 5) multipler = 5;
            if (multipler < 0.1) multipler = 0.1;
            lock (EntityLock)
            {
                for (int i = 0; i < Entities.Count; i++)
                {
                    var entity = Entities[i];
                    if (entity.BeginUpdate())
                    {
                        entity.Velocity -= new Vector3(0, entity.AccelerationDueToGravity * multipler, 0);
                        entity.Velocity.Clamp(entity.TerminalVelocity);
                        if (entity is IAABBEntity)
                            CheckWithTerrain((IAABBEntity)entity, World);
                        entity.EndUpdate(entity.Position + entity.Velocity);
                    }
                }
            }
            LastUpdate = DateTime.UtcNow;
        }

        private void CheckWithTerrain(IAABBEntity entity, ReadOnlyWorld world)
        {
            Vector3 collisionPoint, collisionDirection;
            if (entity.Position.Y > 0 && entity.Position.Y <= 127) // Don't do checks outside the map
            {
                bool fireEvent = entity.Velocity != Vector3.Zero;
                // Do terrain collisions
                if (AdjustVelocityX(entity, world, out collisionPoint, out collisionDirection))
                {
                    if (fireEvent)
                        entity.TerrainCollision(collisionPoint, collisionDirection);
                }
                if (AdjustVelocityY(entity, world, out collisionPoint, out collisionDirection))
                {
                    entity.Velocity *= new Vector3(0.1, 1, 0.1); // TODO: More sophisticated friction
                    if (fireEvent)
                        entity.TerrainCollision(collisionPoint, collisionDirection);
                }
                if (AdjustVelocityZ(entity, world, out collisionPoint, out collisionDirection))
                {
                    if (fireEvent)
                        entity.TerrainCollision(collisionPoint, collisionDirection);
                }
            }
        }

        // TODO: There's a lot of code replication here, perhaps it can be consolidated
        /// <summary>
        /// Performs terrain collision tests and adjusts the X-axis velocity accordingly
        /// </summary>
        /// <returns>True if the entity collides with the terrain</returns>
        private bool AdjustVelocityX(IAABBEntity entity, ReadOnlyWorld world, out Vector3 collision, out Vector3 collisionDirection)
        {
            collision = Vector3.Zero;
            collisionDirection = Vector3.Zero;
            if (entity.Velocity.X == 0)
                return false;
            // Do some enviornment guessing to improve speed
            int minY = (int)entity.Position.Y - (entity.Position.Y < 0 ? 1 : 0);
            int maxY = (int)(entity.Position.Y + entity.Size.Width) - (entity.Position.Y < 0 ? 1 : 0);
            int minZ = (int)entity.Position.Z - (entity.Position.Z < 0 ? 1 : 0);
            int maxZ = (int)(entity.Position.Z + entity.Size.Depth) - (entity.Position.Z < 0 ? 1 : 0);
            int minX, maxX;

            // Expand bounding box to include area to be tested
            if (entity.Velocity.X < 0)
            {
                TempBoundingBox = new BoundingBox(
                    new Vector3(entity.BoundingBox.Min.X + entity.Velocity.X, entity.BoundingBox.Min.Y, entity.BoundingBox.Min.Z),
                    entity.BoundingBox.Max);

                maxX = (int)(TempBoundingBox.Max.X);
                minX = (int)(TempBoundingBox.Min.X + entity.Velocity.X) - 1;
            }
            else
            {
                TempBoundingBox = new BoundingBox(
                    entity.BoundingBox.Min,
                    new Vector3(entity.BoundingBox.Max.X + entity.Velocity.X, entity.BoundingBox.Max.Y, entity.BoundingBox.Max.Z));
                minX = (int)(entity.BoundingBox.Min.X);
                maxX = (int)(entity.BoundingBox.Max.X + entity.Velocity.X) + 1;
            }

            // Do terrain checks
            double? collisionPoint = null;
            BoundingBox blockBox;
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    for (int z = minZ; z <= maxZ; z++)
                    {
                        var position = new Coordinates3D(x, y, z);
                        var boundingBox = BlockPhysicsProvider.GetBoundingBox(world.World, position);
                        if (boundingBox == null)
                            continue;
                        blockBox = boundingBox.Value.OffsetBy(position + new Vector3(0.5));
                        if (TempBoundingBox.Intersects(blockBox))
                        {
                            if (entity.Velocity.X < 0)
                            {
                                if (!collisionPoint.HasValue)
                                    collisionPoint = blockBox.Max.X;
                                else if (collisionPoint.Value < blockBox.Max.X)
                                    collisionPoint = blockBox.Max.X;
                            }
                            else
                            {
                                if (!collisionPoint.HasValue)
                                    collisionPoint = blockBox.Min.X;
                                else if (collisionPoint.Value > blockBox.Min.X)
                                    collisionPoint = blockBox.Min.X;
                            }
                            collision = position;
                        }
                    }
                }
            }

            if (collisionPoint != null)
            {
                if (entity.Velocity.X < 0)
                {
                    entity.Velocity = new Vector3(
                        entity.Velocity.X - (TempBoundingBox.Min.X - collisionPoint.Value),
                        entity.Velocity.Y,
                        entity.Velocity.Z);
                    collisionDirection = Vector3.Left;
                }
                else if (entity.Velocity.X > 0)
                {
                    entity.Velocity = new Vector3(
                        entity.Velocity.X - (TempBoundingBox.Max.X - collisionPoint.Value),
                        entity.Velocity.Y,
                        entity.Velocity.Z);
                    collisionDirection = Vector3.Right;
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Performs terrain collision tests and adjusts the Y-axis velocity accordingly
        /// </summary>
        /// <returns>True if the entity collides with the terrain</returns>
        private bool AdjustVelocityY(IAABBEntity entity, ReadOnlyWorld world, out Vector3 collision, out Vector3 collisionDirection)
        {
            collision = Vector3.Zero;
            collisionDirection = Vector3.Zero;
            if (entity.Velocity.Y == 0)
                return false;
            // Do some enviornment guessing to improve speed
            int minX = (int)entity.Position.X - (entity.Position.X < 0 ? 1 : 0);
            int maxX = (int)(entity.Position.X + entity.Size.Width) - (entity.Position.X < 0 ? 1 : 0);
            int minZ = (int)entity.Position.Z - (entity.Position.Z < 0 ? 1 : 0);
            int maxZ = (int)(entity.Position.Z + entity.Size.Depth) - (entity.Position.Z < 0 ? 1 : 0);
            int minY, maxY;

            // Expand bounding box to include area to be tested
            if (entity.Velocity.Y < 0)
            {
                TempBoundingBox = new BoundingBox(
                    new Vector3(entity.BoundingBox.Min.X, entity.BoundingBox.Min.Y + entity.Velocity.Y, entity.BoundingBox.Min.Z),
                    entity.BoundingBox.Max);

                maxY = (int)(TempBoundingBox.Max.Y);
                minY = (int)(TempBoundingBox.Min.Y + entity.Velocity.Y) - 1;
            }
            else
            {
                TempBoundingBox = new BoundingBox(
                    entity.BoundingBox.Min,
                    new Vector3(entity.BoundingBox.Max.X, entity.BoundingBox.Max.Y + entity.Velocity.Y, entity.BoundingBox.Max.Z));
                minY = (int)(entity.BoundingBox.Min.Y);
                maxY = (int)(entity.BoundingBox.Max.Y + entity.Velocity.Y) + 1;
            }

            // Clamp Y into map boundaries
            if (minY < 0)
                minY = 0;
            if (minY >= TrueCraft.Core.World.World.Height)
                minY = TrueCraft.Core.World.World.Height - 1;
            if (maxY < 0)
                maxY = 0;
            if (maxY >= TrueCraft.Core.World.World.Height)
                maxY = TrueCraft.Core.World.World.Height - 1;

            // Do terrain checks
            double? collisionPoint = null;
            BoundingBox blockBox;
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    for (int z = minZ; z <= maxZ; z++)
                    {
                        var position = new Coordinates3D(x, y, z);
                        if (!World.IsValidPosition(position))
                            continue;
                        var boundingBox = BlockPhysicsProvider.GetBoundingBox(world.World, position);
                        if (boundingBox == null)
                            continue;
                        blockBox = boundingBox.Value.OffsetBy(position + new Vector3(0.5));
                        if (TempBoundingBox.Intersects(blockBox))
                        {
                            if (entity.Velocity.Y < 0)
                            {
                                if (!collisionPoint.HasValue)
                                    collisionPoint = blockBox.Max.Y;
                                else if (collisionPoint.Value < blockBox.Max.Y)
                                    collisionPoint = blockBox.Max.Y;
                            }
                            else
                            {
                                if (!collisionPoint.HasValue)
                                    collisionPoint = blockBox.Min.Y;
                                else if (collisionPoint.Value > blockBox.Min.Y)
                                    collisionPoint = blockBox.Min.Y;
                            }
                            collision = position;
                        }
                    }
                }
            }

            if (collisionPoint != null)
            {
                if (entity.Velocity.Y < 0)
                {
                    // TODO: Do block event
                    //var block = world.GetBlock(collision);
                    //block.OnBlockWalkedOn(world, collision, this);
                    entity.Velocity = new Vector3(entity.Velocity.X,
                        entity.Velocity.Y + (collisionPoint.Value - TempBoundingBox.Min.Y),
                        entity.Velocity.Z);
                    collisionDirection = Vector3.Down;
                }
                else if (entity.Velocity.Y > 0)
                {
                    entity.Velocity = new Vector3(entity.Velocity.X,
                        entity.Velocity.Y - (TempBoundingBox.Max.Y - collisionPoint.Value),
                        entity.Velocity.Z);
                    collisionDirection = Vector3.Up;
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Performs terrain collision tests and adjusts the Z-axis velocity accordingly
        /// </summary>
        /// <returns>True if the entity collides with the terrain</returns>
        private bool AdjustVelocityZ(IAABBEntity entity, ReadOnlyWorld world, out Vector3 collision, out Vector3 collisionDirection)
        {
            collision = Vector3.Zero;
            collisionDirection = Vector3.Zero;
            if (entity.Velocity.Z == 0)
                return false;
            // Do some enviornment guessing to improve speed
            int minX = (int)entity.Position.X - (entity.Position.X < 0 ? 1 : 0);
            int maxX = (int)(entity.Position.X + entity.Size.Depth) - (entity.Position.X < 0 ? 1 : 0);
            int minY = (int)entity.Position.Y - (entity.Position.Y < 0 ? 1 : 0);
            int maxY = (int)(entity.Position.Y + entity.Size.Width) - (entity.Position.Y < 0 ? 1 : 0);
            int minZ, maxZ;

            // Expand bounding box to include area to be tested
            if (entity.Velocity.Z < 0)
            {
                TempBoundingBox = new BoundingBox(
                    new Vector3(entity.BoundingBox.Min.X, entity.BoundingBox.Min.Y, entity.BoundingBox.Min.Z + entity.Velocity.Z),
                    entity.BoundingBox.Max);

                maxZ = (int)(TempBoundingBox.Max.Z);
                minZ = (int)(TempBoundingBox.Min.Z + entity.Velocity.Z) - 1;
            }
            else
            {
                TempBoundingBox = new BoundingBox(
                    entity.BoundingBox.Min,
                    new Vector3(entity.BoundingBox.Max.X, entity.BoundingBox.Max.Y, entity.BoundingBox.Max.Z + entity.Velocity.Z)
                );
                minZ = (int)(entity.BoundingBox.Min.Z);
                maxZ = (int)(entity.BoundingBox.Max.Z + entity.Velocity.Z) + 1;
            }

            // Do terrain checks
            double? collisionPoint = null;
            BoundingBox blockBox;
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    for (int z = minZ; z <= maxZ; z++)
                    {
                        var position = new Coordinates3D(x, y, z);
                        var boundingBox = BlockPhysicsProvider.GetBoundingBox(world.World, position);
                        if (boundingBox == null)
                            continue;
                        blockBox = boundingBox.Value.OffsetBy(position + new Vector3(0.5));
                        if (TempBoundingBox.Intersects(blockBox))
                        {
                            if (entity.Velocity.Z < 0)
                            {
                                if (!collisionPoint.HasValue)
                                    collisionPoint = blockBox.Max.Z;
                                else if (collisionPoint.Value < blockBox.Max.Z)
                                    collisionPoint = blockBox.Max.Z;
                            }
                            else
                            {
                                if (!collisionPoint.HasValue)
                                    collisionPoint = blockBox.Min.Z;
                                else if (collisionPoint.Value > blockBox.Min.Z)
                                    collisionPoint = blockBox.Min.Z;
                            }
                            collision = position;
                        }
                    }
                }
            }

            if (collisionPoint != null)
            {
                if (entity.Velocity.Z < 0)
                {
                    entity.Velocity = new Vector3(
                        entity.Velocity.X,
                        entity.Velocity.Y,
                        entity.Velocity.Z - (TempBoundingBox.Min.Z - collisionPoint.Value));
                    collisionDirection = Vector3.Backwards;
                }
                else if (entity.Velocity.Z > 0)
                {
                    entity.Velocity = new Vector3(
                        entity.Velocity.X,
                        entity.Velocity.Y,
                        entity.Velocity.Z - (TempBoundingBox.Max.Z - collisionPoint.Value));
                    collisionDirection = Vector3.Forwards;
                }
                return true;
            }

            return false;
        }
    }
}
