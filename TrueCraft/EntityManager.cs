using System;
using TrueCraft.API.Server;
using TrueCraft.API.World;
using TrueCraft.API.Entities;
using TrueCraft.API.Networking;
using System.ComponentModel;
using TrueCraft.Core.Entities;
using System.Linq;
using System.Threading.Tasks;
using TrueCraft.Core.Networking.Packets;
using TrueCraft.Core;
using System.Collections.Generic;
using TrueCraft.Core.World;
using TrueCraft.API;
using System.Collections.Concurrent;

namespace TrueCraft
{
    public class EntityManager : IEntityManager
    {
        public IWorld World { get; set; }
        public IMultiplayerServer Server { get; set; }
        public PhysicsEngine PhysicsEngine { get; set; }

        private int NextEntityID { get; set; }
        private List<IEntity> Entities { get; set; } // TODO: Persist to disk
        private object EntityLock = new object();
        private ConcurrentBag<IEntity> PendingDespawns { get; set; }

        private static readonly int MaxClientDistance = 4;

        public EntityManager(IMultiplayerServer server, IWorld world)
        {
            Server = server;
            World = world;
            PhysicsEngine = new PhysicsEngine(world, (BlockRepository)server.BlockRepository);
            PendingDespawns = new ConcurrentBag<IEntity>();
            Entities = new List<IEntity>();
            // TODO: Handle loading worlds that already have entities
            // Note: probably not the concern of EntityManager. The server could manually set this?
            NextEntityID = 1;
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var entity = sender as IEntity;
            if (entity == null)
                throw new InvalidCastException("Attempted to handle property changes for non-entity");
            if (entity is PlayerEntity)
                HandlePlayerPropertyChanged(e.PropertyName, entity as PlayerEntity);
            switch (e.PropertyName)
            {
                case "Position":
                case "Yaw":
                case "Pitch":
                    PropegateEntityPositionUpdates(entity);
                    break;
            }
        }

        private void HandlePlayerPropertyChanged(string property, PlayerEntity entity)
        {
            var client = GetClientForEntity(entity) as RemoteClient;
            if (client == null)
                return; // Note: would an exception be appropriate here?
            switch (property)
            {
                case "Position":
                    // TODO: Only trigger this if the player crosses a chunk boundary
                    Task.Factory.StartNew(client.UpdateChunks);
                    break;
            }
        }

        private void PropegateEntityPositionUpdates(IEntity entity)
        {
            for (int i = 0, ServerClientsCount = Server.Clients.Count; i < ServerClientsCount; i++)
            {
                var client = Server.Clients[i] as RemoteClient;
                if (client.Entity == entity)
                    continue; // Do not send movement updates back to the client that triggered them
                if (client.KnownEntities.Contains(entity))
                {
                    // TODO: Consider using more kinds of entity packets (i.e. EntityRelativeMovePacket) that may be more effecient
                    // In the past I've done this and entity positions quickly got very inaccurate on the client.
                    client.QueuePacket(new EntityTeleportPacket(entity.EntityID,
                        MathHelper.CreateAbsoluteInt(entity.Position.X),
                        MathHelper.CreateAbsoluteInt(entity.Position.Y),
                        MathHelper.CreateAbsoluteInt(entity.Position.Z),
                        MathHelper.CreateRotationByte(entity.Yaw),
                        MathHelper.CreateRotationByte(entity.Pitch)));
                }
            }
        }

        private bool IsInRange(Vector3 a, Vector3 b, int range)
        {
            return Math.Abs(a.X - b.X) < range * Chunk.Width &&
                Math.Abs(a.Z - b.Z) < range * Chunk.Depth;
        }

        private IEntity[] GetEntitiesInRange(IEntity entity, int maxChunks)
        {
            return Entities.Where(e => e != entity && IsInRange(e.Position, entity.Position, maxChunks)).ToArray();
        }

        IRemoteClient GetClientForEntity(PlayerEntity entity)
        {
            return Server.Clients.SingleOrDefault(c => c.Entity != null && c.Entity.EntityID == entity.EntityID);
        }

        public IList<IEntity> EntitiesInRange(Vector3 center, float radius)
        {
            return Entities.Where(e => e.Position.DistanceTo(center) < radius).ToList();
        }

        public IList<IRemoteClient> ClientsForEntity(IEntity entity)
        {
            return Server.Clients.Where(c => (c as RemoteClient).KnownEntities.Contains(entity)).ToList();
        }

        public void SpawnEntity(IEntity entity)
        {
            entity.EntityID = NextEntityID++;
            entity.PropertyChanged -= HandlePropertyChanged;
            entity.PropertyChanged += HandlePropertyChanged;
            lock (EntityLock)
            {
                Entities.Add(entity);
            }
            foreach (var clientEntity in GetEntitiesInRange(entity, MaxClientDistance))
            {
                if (clientEntity != entity && clientEntity is PlayerEntity)
                {
                    var client = (RemoteClient)GetClientForEntity((PlayerEntity)clientEntity);
                    client.KnownEntities.Add(entity);
                    client.QueuePacket(entity.SpawnPacket);
                }
            }
            if (entity is IPhysicsEntity)
                PhysicsEngine.AddEntity(entity as IPhysicsEntity);
        }

        public void DespawnEntity(IEntity entity)
        {
            PendingDespawns.Add(entity);
        }

        public IEntity GetEntityByID(int id)
        {
            return Entities.SingleOrDefault(e => e.EntityID == id);
        }

        public void Update()
        {
            PhysicsEngine.Update();
            var updates = Parallel.ForEach(Entities, e => e.Update(this));
            while (!updates.IsCompleted);
            IEntity entity;
            while (PendingDespawns.Count != 0)
            {
                while (!PendingDespawns.TryTake(out entity));
                for (int i = 0, ServerClientsCount = Server.Clients.Count; i < ServerClientsCount; i++)
                {
                    var client = (RemoteClient)Server.Clients[i];
                    if (client.KnownEntities.Contains(entity))
                    {
                        client.QueuePacket(new DestroyEntityPacket(entity.EntityID));
                    }
                }
                lock (EntityLock)
                {
                    Entities.Remove(entity);
                }
            }
        }

        /// <summary>
        /// Performs the initial population of client entities.
        /// </summary>
        public void SendEntitiesToClient(IRemoteClient _client)
        {
            var client = _client as RemoteClient;
            foreach (var entity in GetEntitiesInRange(client.Entity, MaxClientDistance))
            {
                if (entity != client.Entity)
                {
                    client.KnownEntities.Add(entity);
                    client.QueuePacket(entity.SpawnPacket);
                }
            }
        }
    }
}