using System;
using TrueCraft.API.Networking;
using TrueCraft.Core.Networking.Packets;
using TrueCraft.API.Entities;
using TrueCraft.API;
using TrueCraft.API.Server;
using System.Linq;

namespace TrueCraft.Core.Entities
{
    public abstract class MobEntity : LivingEntity, IAABBEntity
    {
        public MobEntity()
        {
            Speed = 1; // TODO: WTF
        }

        public override IPacket SpawnPacket
        {
            get
            {
                return new SpawnMobPacket(EntityID, MobType,
                    MathHelper.CreateAbsoluteInt(Position.X),
                    MathHelper.CreateAbsoluteInt(Position.Y),
                    MathHelper.CreateAbsoluteInt(Position.Z),
                    MathHelper.CreateRotationByte(Yaw),
                    MathHelper.CreateRotationByte(Pitch),
                    Metadata);
            }
        }

        public abstract sbyte MobType { get; }

        public virtual void TerrainCollision(Vector3 collisionPoint, Vector3 collisionDirection)
        {
            // This space intentionally left blank
        }

        public BoundingBox BoundingBox
        {
            get
            {
                return new BoundingBox(Position, Position + Size);
            }
        }

        public virtual bool BeginUpdate()
        {
            EnablePropertyChange = false;
            return true;
        }

        public virtual void EndUpdate(Vector3 newPosition)
        {
            EnablePropertyChange = true;
            Position = newPosition;
        }

        public float AccelerationDueToGravity
        {
            get
            {
                return 0.08f;
            }
        }

        public float Drag
        {
            get
            {
                return 0.02f;
            }
        }

        public float TerminalVelocity
        {
            get
            {
                return 3.92f;
            }
        }

        public PathResult CurrentPath { get; set; }

        /// <summary>
        /// Mob's current speed in m/s.
        /// </summary>
        public virtual double Speed { get; }

        protected DateTime CurrentNodeStart = DateTime.MinValue;

        public void Face(Vector3 target)
        {
            var diff = target - Position;
            Yaw = (float)MathHelper.RadiansToDegrees(-(Math.Atan2(diff.X, diff.Z) - Math.PI) + Math.PI); // "Flip" over the 180 mark
        }

        protected void AdvancePath(bool faceRoute = true)
        {
            if (CurrentPath != null)
            {
                if (CurrentNodeStart == DateTime.MinValue)
                    CurrentNodeStart = DateTime.UtcNow;
                // Advance along path
                var target = (Vector3)CurrentPath.Waypoints[CurrentPath.Index];
                var diff = target - Position;
                var max = (DateTime.UtcNow - CurrentNodeStart).TotalSeconds * Speed;
                if (faceRoute)
                    Face(target);
                diff.Clamp(max);
                Position += diff;
                if (Position == target)
                {
                    CurrentPath.Index++;
                    if (CurrentPath.Index >= CurrentPath.Waypoints.Count)
                    {
                        CurrentPath = null;
                        CurrentNodeStart = DateTime.MinValue;
                        // TODO: Raise path complete event or something?
                    }
                }
            }
        }

        public override void Update(IEntityManager entityManager)
        {
            AdvancePath();
            base.Update(entityManager);
        }
    }
}

