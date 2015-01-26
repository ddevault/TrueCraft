using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class GoldenShovelItem : ShovelItem
    {
        public static readonly short ItemID = 0x11C;

        public override short ID { get { return 0x11C; } }

        public override ToolMaterial Material { get { return ToolMaterial.Gold; } }

        public override short BaseDurability { get { return 33; } }

        public override string DisplayName { get { return "Golden Shovel"; } }
    }
}