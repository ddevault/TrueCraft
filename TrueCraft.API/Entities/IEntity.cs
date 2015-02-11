using System;
using System.ComponentModel;
using TrueCraft.API.Networking;
using TrueCraft.API.Server;
using TrueCraft.API.World;

namespace TrueCraft.API.Entities
{
    public interface IEntity : INotifyPropertyChanged
    {
        IPacket SpawnPacket { get; }
        int EntityID { get; set; }
        Vector3 Position { get; set; }
        float Yaw { get; set; }
        float Pitch { get; set; }
        bool Despawned { get; set; }
        DateTime SpawnTime { get; set; }
        MetadataDictionary Metadata { get; }
        Size Size { get; }
        IEntityManager EntityManager { get; set; }
        IWorld World { get; set; }
        bool SendMetadataToClients { get; }
        void Update(IEntityManager entityManager);
    }
}