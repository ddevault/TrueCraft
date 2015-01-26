using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public abstract class ArmourItem : ItemProvider
    {
        public abstract ArmourMaterial Material { get; }

        public virtual short BaseDurability { get { return 0; } }

        public abstract float BaseArmour { get; }

        public override sbyte MaximumStack { get { return 1; } }
    }
}
