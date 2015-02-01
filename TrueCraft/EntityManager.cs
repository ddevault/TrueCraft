using System;
using TrueCraft.API.Server;
using TrueCraft.API.World;
using TrueCraft.API.Entities;
using TrueCraft.API.Networking;
using System.ComponentModel;
using TrueCraft.Entities;
using System.Linq;
using System.Threading.Tasks;
using TrueCraft.Core.Networking.Packets;
using TrueCraft.Core;

namespace TrueCraft
{
    public class EntityManager : IEntityManager
    {
        public IWorld World { get; set; }
        public IMultiplayerServer Server { get; set; }

        private int NextEntityID { get; set; }

        public EntityManager(IMultiplayerServer server, IWorld world)
        {
            Server = server;
            World = world;
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

        IRemoteClient GetClientForEntity(PlayerEntity entity)
        {
            return Server.Clients.SingleOrDefault(c => c.Entity.EntityID == entity.EntityID);
        }

        public void SpawnEntity(IEntity entity)
        {
            entity.EntityID = NextEntityID++;
            entity.PropertyChanged -= HandlePropertyChanged;
            entity.PropertyChanged += HandlePropertyChanged;
        }

        public void DespawnEntity(IEntity entity)
        {
            throw new NotImplementedException();
        }

        public IEntity GetEntityByID(int id)
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public void SendEntitiesToClient(IRemoteClient client)
        {
            throw new NotImplementedException();
        }
    }
}