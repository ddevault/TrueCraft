using System;
using TrueCraft.API.Physics;

namespace TrueCraft.API.Entities
{
    public interface IMobEntity : IEntity, IAABBEntity
    {
        event EventHandler PathComplete;
        PathResult CurrentPath { get; set; }
        bool AdvancePath(TimeSpan time, bool faceRoute = true);
        void Face(Vector3 target);
    }
}
