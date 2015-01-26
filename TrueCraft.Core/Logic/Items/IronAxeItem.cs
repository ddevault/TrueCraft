using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class IronAxeItem : ToolItem
    {
        public static readonly short ItemID = 0x102;

        public override short ID { get { return 0x102; } }

        public override ToolMaterial Material { get { return ToolMaterial.Iron; } }

        public override string DisplayName { get { return "Iron Axe"; } }
    }
}