using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API;

namespace TrueCraft.Core.Logic
{
    public abstract class ToolItem : ItemProvider
    {
        public abstract ToolMaterial Material { get; }

        public override sbyte MaximumStack { get { return 1; } }
    }
}
