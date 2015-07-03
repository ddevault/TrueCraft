using System;
using TrueCraft.API.Server;
using TrueCraft.API.Entities;

namespace TrueCraft.API.AI
{
    public interface IMobState
    {
        void Update(IMobEntity entity, IEntityManager manager);
    }
}