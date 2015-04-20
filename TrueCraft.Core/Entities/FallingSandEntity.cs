using System;
using TrueCraft.API;
using TrueCraft.API.Networking;
using TrueCraft.Core.Networking.Packets;
using TrueCraft.API.Entities;
using TrueCraft.API.Server;
using TrueCraft.API.World;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.Entities
{
    public class FallingSandEntity : ObjectEntity, IAABBEntity
    {
        public FallingSandEntity(Vector3 position)
        {
            _Position = position + new Vector3(0.5);
        }

        public override byte EntityType { get { return 70; } }

        public override Size Size
        {
            get
            {
                return new Size(1);
            }
        }

        public override IPacket SpawnPacket
        {
            get
            {
                return new SpawnGenericEntityPacket(EntityID, (sbyte)EntityType,
                    MathHelper.CreateAbsoluteInt(Position.X), MathHelper.CreateAbsoluteInt(Position.Y),
                    MathHelper.CreateAbsoluteInt(Position.Z), 0, null, null, null);
            }
        }

        public override int Data { get { return 1; } }

        public void TerrainCollision(Vector3 collisionPoint, Vector3 collisionDirection)
        {
            if (Despawned)
                return;
            if (collisionDirection == Vector3.Down)
            {
                var id = SandBlock.BlockID;
                if (EntityType == 71)
                    id = GravelBlock.BlockID;
                EntityManager.DespawnEntity(this);
                World.SetBlockID((Coordinates3D)_Position, id);
            }
        }

        public BoundingBox BoundingBox
        {
            get
            {
                return new BoundingBox(Position, Position + Size);
            }
        }

        public bool BeginUpdate()
        {
            EnablePropertyChange = false;
            return true;
        }

        public void EndUpdate(Vector3 newPosition)
        {
            EnablePropertyChange = true;
            Position = newPosition;
        }

        public float AccelerationDueToGravity
        {
            get
            {
                return 0.04f;
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
                return 1.96f;
            }
        }
    }
}