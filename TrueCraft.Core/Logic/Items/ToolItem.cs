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

        public virtual ToolType ToolType { get { return ToolType.None; } }

        public virtual short BaseDurability { get { return 0; } }

        public override sbyte MaximumStack { get { return 1; } }

        public virtual int Uses
        {
            get
            {
                switch (Material)
                {
                    case ToolMaterial.Gold:
                        return 33;
                    case ToolMaterial.Wood:
                        return 60;
                    case ToolMaterial.Stone:
                        return 132;
                    case ToolMaterial.Iron:
                        return 251;
                    case ToolMaterial.Diamond:
                        return 1562;
                    default:
                        return -1;
                }
            }
        }
    }
}