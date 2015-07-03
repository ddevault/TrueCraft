using System;

namespace TrueCraft.API.Entities
{
    public interface IMobEntity : IEntity, IAABBEntity
    {
        PathResult CurrentPath { get; set; }
        bool AdvancePath(TimeSpan time, bool faceRoute = true);
        void Face(Vector3 target);
    }
}