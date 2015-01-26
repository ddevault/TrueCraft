using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic
{
    public abstract class ItemProvider : IItemProvider
    {
        public abstract short ID { get; }

        public virtual sbyte MaximumStack { get { return 64; } }

        public virtual string DisplayName { get { return string.Empty; } }
    }
}