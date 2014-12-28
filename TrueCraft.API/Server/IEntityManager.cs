using System;
using TrueCraft.API.Entities;
using System.Collections.Generic;
using TrueCraft.API.Networking;

namespace TrueCraft.API.Server
{
    public interface IEntityManager
    {
        /// <summary>
        /// Adds an entity to the world and assigns it an entity ID.
        /// </summary>
        void SpawnEntity(IEntity entity);
        void DespawnEntity(IEntity entity);
        IEntity GetEntityByID(int id);
        void Update();
        void SendEntitiesToClient(IRemoteClient client);
    }
}