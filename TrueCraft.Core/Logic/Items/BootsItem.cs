using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class LeatherBootsItem : ArmourItem
    {
        public static readonly short ItemID = 0x12D;

        public override short ID { get { return 0x12D; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Leather; } }

        public override short BaseDurability { get { return 40; } }

        public override float BaseArmour { get { return 1.5f; } }

        public override string DisplayName { get { return "Leather Boots"; } }
    }

    public class IronBootsItem : ArmourItem
    {
        public static readonly short ItemID = 0x135;

        public override short ID { get { return 0x135; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Iron; } }

        public override short BaseDurability { get { return 160; } }

        public override float BaseArmour { get { return 1.5f; } }

        public override string DisplayName { get { return "Iron Boots"; } }
    }

    public class GoldenBootsItem : ArmourItem
    {
        public static readonly short ItemID = 0x13D;

        public override short ID { get { return 0x13D; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Gold; } }

        public override short BaseDurability { get { return 80; } }

        public override float BaseArmour { get { return 1.5f; } }

        public override string DisplayName { get { return "Golden Boots"; } }
    }

    public class DiamondBootsItem : ArmourItem
    {
        public static readonly short ItemID = 0x139;

        public override short ID { get { return 0x139; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Diamond; } }

        public override short BaseDurability { get { return 320; } }

        public override float BaseArmour { get { return 1.5f; } }

        public override string DisplayName { get { return "Diamond Boots"; } }
    }

    public class ChainBootsItem : ArmourItem
    {
        public static readonly short ItemID = 0x131;

        public override short ID { get { return 0x131; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Chain; } }

        public override short BaseDurability { get { return 79; } }

        public override float BaseArmour { get { return 1.5f; } }

        public override string DisplayName { get { return "Chain Boots"; } }
    }
}