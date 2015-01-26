using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public abstract class SwordItem : ToolItem
    {
        public abstract float Damage { get; }
    }

    public class WoodenSwordItem : SwordItem
    {
        public static readonly short ItemID = 0x10C;

        public override short ID { get { return 0x10C; } }

        public override ToolMaterial Material { get { return ToolMaterial.Wood; } }

        public override short BaseDurability { get { return 60; } }

        public override float Damage { get { return 2.5f; } }

        public override string DisplayName { get { return "Wooden Sword"; } }
    }

    public class StoneSwordItem : SwordItem
    {
        public static readonly short ItemID = 0x110;

        public override short ID { get { return 0x110; } }

        public override ToolMaterial Material { get { return ToolMaterial.Stone; } }

        public override short BaseDurability { get { return 132; } }

        public override float Damage { get { return 3.5f; } }

        public override string DisplayName { get { return "Stone Sword"; } }
    }

    public class IronSwordItem : SwordItem
    {
        public static readonly short ItemID = 0x10B;

        public override short ID { get { return 0x10B; } }

        public override ToolMaterial Material { get { return ToolMaterial.Iron; } }

        public override short BaseDurability { get { return 251; } }

        public override float Damage { get { return 4.5f; } }

        public override string DisplayName { get { return "Iron Sword"; } }
    }

    public class GoldenSwordItem : SwordItem
    {
        public static readonly short ItemID = 0x11B;

        public override short ID { get { return 0x11B; } }

        public override ToolMaterial Material { get { return ToolMaterial.Gold; } }

        public override short BaseDurability { get { return 33; } }

        public override float Damage { get { return 2.5f; } }

        public override string DisplayName { get { return "Golden Sword"; } }
    }

    public class DiamondSwordItem : SwordItem
    {
        public static readonly short ItemID = 0x114;

        public override short ID { get { return 0x114; } }

        public override ToolMaterial Material { get { return ToolMaterial.Diamond; } }

        public override short BaseDurability { get { return 1562; } }

        public override float Damage { get { return 5.5f; } }

        public override string DisplayName { get { return "Diamond Sword"; } }
    }
}