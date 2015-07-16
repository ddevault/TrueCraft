using System;
using TrueCraft.API.Networking;
using TrueCraft.Core.Networking.Packets;
using TrueCraft.API.Entities;
using TrueCraft.API;
using TrueCraft.API.Server;
using System.Linq;
using TrueCraft.API.AI;
using TrueCraft.Core.AI;
using TrueCraft.API.Physics;

namespace TrueCraft.Core.Entities
{
    public abstract class MobEntity : LivingEntity, IAABBEntity, IMobEntity
    {
        protected MobEntity()
        {
            Speed = 4;
            CurrentState = new WanderState();
        }

        public event EventHandler PathComplete;

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

        public virtual bool Friendly { get { return true; } }

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
                return 32f;
            }
        }

        public float Drag
        {
            get
            {
                return 0.40f;
            }
        }

        public float TerminalVelocity
        {
            get
            {
                return 78.4f;
            }
        }

        public PathResult CurrentPath { get; set; }

        /// <summary>
        /// Mob's current speed in m/s.
        /// </summary>
        public virtual double Speed { get; set; }

        public IMobState CurrentState { get; set; }

        public void ChangeState(IMobState state)
        {
            CurrentState = state;
        }

        public void Face(Vector3 target)
        {
            var diff = target - Position;
            Yaw = (float)MathHelper.RadiansToDegrees(-(Math.Atan2(diff.X, diff.Z) - Math.PI) + Math.PI); // "Flip" over the 180 mark
        }

        public bool AdvancePath(TimeSpan time, bool faceRoute = true)
        {
            var modifier = time.TotalSeconds * Speed;
            if (CurrentPath != null)
            {
                // Advance along path
                var target = (Vector3)CurrentPath.Waypoints[CurrentPath.Index];
                target.Y = Position.Y; // TODO: Find better way of doing this
                var diff = target - Position;
                diff *= modifier;
                if (faceRoute)
                    Face(target);
                Position += diff;
                if (Position.DistanceTo(target) < 0.1)
                {
                    CurrentPath.Index++;
                    if (CurrentPath.Index >= CurrentPath.Waypoints.Count)
                    {
                        CurrentPath = null;
                        if (PathComplete != null)
                            PathComplete(this, null);
                        return true;
                    }
                }
            }
            return false;
        }

        public override void Update(IEntityManager entityManager)
        {
            if (CurrentState != null)
                CurrentState.Update(this, entityManager);
            else
                AdvancePath(entityManager.TimeSinceLastUpdate);
            base.Update(entityManager);
        }
    }
}

