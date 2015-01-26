using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public abstract class HoeItem : ToolItem
    {
    }

    public class WoodenHoeItem : HoeItem
    {
        public static readonly short ItemID = 0x122;

        public override short ID { get { return 0x122; } }

        public override ToolMaterial Material { get { return ToolMaterial.Wood; } }

        public override short BaseDurability { get { return 60; } }

        public override string DisplayName { get { return "Wooden Hoe"; } }
    }

    public class StoneHoeItem : HoeItem
    {
        public static readonly short ItemID = 0x123;

        public override short ID { get { return 0x123; } }

        public override ToolMaterial Material { get { return ToolMaterial.Stone; } }

        public override short BaseDurability { get { return 132; } }

        public override string DisplayName { get { return "Stone Hoe"; } }
    }

    public class IronHoeItem : HoeItem
    {
        public static readonly short ItemID = 0x124;

        public override short ID { get { return 0x124; } }

        public override ToolMaterial Material { get { return ToolMaterial.Iron; } }

        public override short BaseDurability { get { return 251; } }

        public override string DisplayName { get { return "Iron Hoe"; } }
    }

    public class GoldenHoeItem : HoeItem
    {
        public static readonly short ItemID = 0x126;

        public override short ID { get { return 0x126; } }

        public override ToolMaterial Material { get { return ToolMaterial.Gold; } }

        public override short BaseDurability { get { return 33; } }

        public override string DisplayName { get { return "Golden Hoe"; } }
    }

    public class DiamondHoeItem : HoeItem
    {
        public static readonly short ItemID = 0x125;

        public override short ID { get { return 0x125; } }

        public override ToolMaterial Material { get { return ToolMaterial.Diamond; } }

        public override short BaseDurability { get { return 1562; } }

        public override string DisplayName { get { return "Diamond Hoe"; } }
    }
}