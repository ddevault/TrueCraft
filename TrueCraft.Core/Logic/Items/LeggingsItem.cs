using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class LeatherPantsItem : ArmourItem
    {
        public static readonly short ItemID = 0x12C;

        public override short ID { get { return 0x12C; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Leather; } }

        public override short BaseDurability { get { return 46; } }

        public override float BaseArmour { get { return 3; } }

        public override string DisplayName { get { return "Leather Pants"; } }
    }

    public class IronLeggingsItem : ArmourItem
    {
        public static readonly short ItemID = 0x134;

        public override short ID { get { return 0x134; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Iron; } }

        public override short BaseDurability { get { return 184; } }

        public override float BaseArmour { get { return 3; } }

        public override string DisplayName { get { return "Iron Leggings"; } }
    }

    public class GoldenLeggingsItem : ArmourItem
    {
        public static readonly short ItemID = 0x13C;

        public override short ID { get { return 0x13C; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Gold; } }

        public override short BaseDurability { get { return 92; } }

        public override float BaseArmour { get { return 3; } }

        public override string DisplayName { get { return "Golden Leggings"; } }
    }

    public class DiamondLeggingsItem : ArmourItem
    {
        public static readonly short ItemID = 0x138;

        public override short ID { get { return 0x138; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Diamond; } }

        public override short BaseDurability { get { return 368; } }

        public override float BaseArmour { get { return 3; } }

        public override string DisplayName { get { return "Diamond Leggings"; } }
    }

    public class ChainLeggingsItem : ArmourItem
    {
        public static readonly short ItemID = 0x130;

        public override short ID { get { return 0x130; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Chain; } }

        public override short BaseDurability { get { return 92; } }

        public override float BaseArmour { get { return 3; } }

        public override string DisplayName { get { return "Chain Leggings"; } }
    }
}