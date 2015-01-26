using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class WoodenHoeItem : HoeItem
    {
        public static readonly short ItemID = 0x122;

        public override short ID { get { return 0x122; } }

        public override ToolMaterial Material { get { return ToolMaterial.Wood; } }

        public override short BaseDurability { get { return 60; } }

        public override string DisplayName { get { return "Wooden Hoe"; } }
    }
}