using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public abstract class ArmorItem : ItemProvider
    {
        public abstract ArmorMaterial Material { get; }

        public virtual short BaseDurability { get { return 0; } }

        public abstract float BaseArmor { get; }

        public override sbyte MaximumStack { get { return 1; } }
    }
}
