using System;
using System.ComponentModel;

namespace TrueCraft.API.Entities
{
    public interface IEntity : INotifyPropertyChanged
    {
        int EntityID { get; set; }
        Vector3 Position { get; set; }
        float Yaw { get; set; }
        float Pitch { get; set; }
        MetadataDictionary Metadata { get; }
        Size Size { get; }
        bool SendMetadataToClients { get; }
    }
}