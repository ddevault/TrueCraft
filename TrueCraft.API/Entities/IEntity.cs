using System;

namespace TrueCraft.API.Entities
{
    public interface IEntity
    {
        int EntityID { get; set; }
        Vector3 Position { get; set; }
    }
}