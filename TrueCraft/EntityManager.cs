using System;
using TrueCraft.API.Server;
using TrueCraft.API.World;
using TrueCraft.API.Entities;
using TrueCraft.API.Networking;

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