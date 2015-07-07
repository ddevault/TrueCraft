using System;
using TrueCraft.API.Entities;
using TrueCraft.API.World;

namespace TrueCraft.API.Physics
{
    public interface IPhysicsEngine
    {
        IWorld World { get; set; }
        void AddEntity(IPhysicsEntity entity);
        void RemoveEntity(IPhysicsEntity entity);
        void Update(TimeSpan time);
    }
}