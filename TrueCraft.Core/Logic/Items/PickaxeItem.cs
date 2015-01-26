using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public abstract class PickaxeItem : ToolItem
    {
    }

    public class WoodenPickaxeItem : PickaxeItem
    {
        public static readonly short ItemID = 0x10E;

        public override short ID { get { return 0x10E; } }

        public override ToolMaterial Material { get { return ToolMaterial.Wood; } }

        public override short BaseDurability { get { return 60; } }

        public override string DisplayName { get { return "Wooden Pickaxe"; } }
    }

    public class StonePickaxeItem : PickaxeItem
    {
        public static readonly short ItemID = 0x112;

        public override short ID { get { return 0x112; } }

        public override ToolMaterial Material { get { return ToolMaterial.Stone; } }

        public override short BaseDurability { get { return 132; } }

        public override string DisplayName { get { return "Stone Pickaxe"; } }
    }

    public class IronPickaxeItem : PickaxeItem
    {
        public static readonly short ItemID = 0x101;

        public override short ID { get { return 0x101; } }

        public override ToolMaterial Material { get { return ToolMaterial.Iron; } }

        public override short BaseDurability { get { return 251; } }

        public override string DisplayName { get { return "Iron Pickaxe"; } }
    }

    public class GoldenPickaxeItem : PickaxeItem
    {
        public static readonly short ItemID = 0x11D;

        public override short ID { get { return 0x11D; } }

        public override ToolMaterial Material { get { return ToolMaterial.Gold; } }

        public override short BaseDurability { get { return 33; } }

        public override string DisplayName { get { return "Golden Pickaxe"; } }
    }

    public class DiamondPickaxeItem : PickaxeItem
    {
        public static readonly short ItemID = 0x116;

        public override short ID { get { return 0x116; } }

        public override ToolMaterial Material { get { return ToolMaterial.Diamond; } }

        public override short BaseDurability { get { return 1562; } }

        public override string DisplayName { get { return "Diamond Pickaxe"; } }
    }
}