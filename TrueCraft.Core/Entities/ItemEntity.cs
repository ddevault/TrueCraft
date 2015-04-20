using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API;
using TrueCraft.API.Networking;
using TrueCraft.API.Entities;
using TrueCraft.Core.Networking.Packets;
using TrueCraft.Core;
using TrueCraft.API.Server;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.Entities
{
    public class ItemEntity : ObjectEntity, IAABBEntity
    {
        public static float PickupRange = 2;

        public ItemEntity(Vector3 position, ItemStack item)
        {
            Position = position;
            Item = item;
            if (item.Empty)
                Despawned = true;
        }

        public ItemStack Item { get; set; }

        public override IPacket SpawnPacket
        {
            get
            {
                return new SpawnItemPacket(EntityID, Item.ID, Item.Count, Item.Metadata,
                    MathHelper.CreateAbsoluteInt(Position.X), MathHelper.CreateAbsoluteInt(Position.Y),
                    MathHelper.CreateAbsoluteInt(Position.Z),
                    MathHelper.CreateRotationByte(Yaw),
                    MathHelper.CreateRotationByte(Pitch), 0);
            }
        }

        public override Size Size
        {
            get { return new Size(0.25, 0.25, 0.25); }
        }

        public BoundingBox BoundingBox
        {
            get
            {
                return new BoundingBox(Position, Position + Size);
            }
        }

        public void TerrainCollision(Vector3 collisionPoint, Vector3 collisionDirection)
        {
            // This space intentionally left blank
        }

        public override byte EntityType
        {
            get { return 2; }
        }

        public override int Data
        {
            get { return 1; }
        }

        public override MetadataDictionary Metadata
        {
            get
            {
                var metadata = base.Metadata;
                metadata[10] = Item;
                return metadata;
            }
        }

        public override bool SendMetadataToClients
        {
            get
            {
                return false;
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

        public override void Update(IEntityManager entityManager)
        {
            var nearbyEntities = entityManager.EntitiesInRange(Position, PickupRange);
            if ((DateTime.Now - SpawnTime).TotalSeconds > 1)
            {
                var player = nearbyEntities.FirstOrDefault(e => e is PlayerEntity && (e as PlayerEntity).Health != 0
                                 && e.Position.DistanceTo(Position) <= PickupRange);
                if (player != null)
                {
                    var playerEntity = player as PlayerEntity;
                    playerEntity.OnPickUpItem(this);
                    entityManager.DespawnEntity(this);
                }
                /* TODO: Merging item entities (this code behaves strangely
                var item = nearbyEntities.FirstOrDefault(e => e is ItemEntity
                    && e != this
                    && (DateTime.Now - (e as ItemEntity).SpawnTime).TotalSeconds > 1
                    && (e as ItemEntity).Item.ID == Item.ID && (e as ItemEntity).Item.Metadata == Item.Metadata
                    && (e as ItemEntity).Item.Nbt == Item.Nbt
                    && e.Position.DistanceTo(Position) < PickupRange);
                if (item != null)
                {
                    // Merge
                    entityManager.DespawnEntity(item);
                    var newItem = Item;
                    newItem.Count += (item as ItemEntity).Item.Count;
                    Item = newItem;
                    OnPropertyChanged("Metadata");
                }*/
            }
            if ((DateTime.Now - SpawnTime).TotalMinutes > 5)
                entityManager.DespawnEntity(this);
            base.Update(entityManager);
        }

        public float AccelerationDueToGravity
        {
            get { return 0.04f; }
        }

        public float Drag
        {
            get { return 0.02f; }
        }

        public float TerminalVelocity
        {
            get { return 1.96f; }
        }
    }
}
