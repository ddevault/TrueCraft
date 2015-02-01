using System;
using TrueCraft.API;
using TrueCraft.API.World;

namespace TrueCraft.API.Entities
{
    public interface IBlockPhysicsProvider
    {
        BoundingBox? GetBoundingBox(IWorld world, Coordinates3D coordinates);
    }
}