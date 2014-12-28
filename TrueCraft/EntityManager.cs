using System;
using TrueCraft.API.Server;
using TrueCraft.API.World;
using TrueCraft.API.Entities;
using TrueCraft.API.Networking;
using System.ComponentModel;
using TrueCraft.Entities;
using System.Linq;
using System.Threading.Tasks;

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
            NextEntityID = 1; // TODO: Handle loading worlds that already have entities
        }

        public void SpawnEntity(IEntity entity)
        {
            entity.EntityID = NextEntityID++;
            entity.PropertyChanged -= HandlePropertyChanged;
            entity.PropertyChanged += HandlePropertyChanged;
        }

        void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var entity = sender as IEntity;
            if (entity == null)
                throw new InvalidCastException("Attempted to handle property changes for non-entity");
            if (entity is PlayerEntity)
            {
                switch (e.PropertyName)
                {
                    case "Position":
                        UpdatePlayerPosition(entity as PlayerEntity);
                        break;
                }
            }
        }

        private void UpdatePlayerPosition(PlayerEntity entity)
        {
            var self = GetClientForEntity(entity);
            for (int i = 0, ServerClientsCount = Server.Clients.Count; i < ServerClientsCount; i++)
            {
                var client = Server.Clients[i];
                if (client == self) continue;
                // TODO: Send updates about
            }
            if (self is RemoteClient)
            {
                Task.Factory.StartNew(() => (self as RemoteClient).UpdateChunks());
            }
        }

        IRemoteClient GetClientForEntity(PlayerEntity entity)
        {
            return Server.Clients.SingleOrDefault(c => c.Entity.EntityID == entity.EntityID);
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