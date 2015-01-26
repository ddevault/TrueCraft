using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API;

namespace TrueCraft.Core.Logic
{
    public abstract class ToolItem : ItemProvider
    {
        public virtual ToolMaterial Material { get { return ToolMaterial.None; } }

        public virtual short BaseDurability { get { return 0; } }

        public override sbyte MaximumStack { get { return 1; } }
    }

    public abstract class AxeItem : ToolItem
    {

    }

    public abstract class HoeItem : ToolItem
    {

    }

    public abstract class PickaxeItem : ToolItem
    {

    }

    public abstract class ShovelItem : ToolItem
    {
    }

    public abstract class SwordItem : ToolItem
    {
        public abstract float Damage { get; }
    }
}
