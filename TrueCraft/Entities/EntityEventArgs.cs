using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.Entities;

namespace TrueCraft.Entities
{
    public class EntityEventArgs : EventArgs
    {
        public IEntity Entity { get; set; }

        public EntityEventArgs(IEntity entity)
        {
            Entity = entity;
        }
    }
}
