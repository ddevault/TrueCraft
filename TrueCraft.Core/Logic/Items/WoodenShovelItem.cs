using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class WoodenShovelItem : ToolItem
    {
        public static readonly short ItemID = 0x10D;

        public override short ID { get { return 0x10D; } }

        public override ToolMaterial Material { get { return ToolMaterial.Wood; } }

        public override string DisplayName { get { return "Wooden Shovel"; } }
    }
}