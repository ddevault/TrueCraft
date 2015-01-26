using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class LeatherTunicItem : ArmourItem
    {
        public static readonly short ItemID = 0x12B;

        public override short ID { get { return 0x12B; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Leather; } }

        public override short BaseDurability { get { return 49; } }

        public override float BaseArmour { get { return 4; } }

        public override string DisplayName { get { return "Leather Tunic"; } }
    }

    public class IronChestplateItem : ArmourItem
    {
        public static readonly short ItemID = 0x133;

        public override short ID { get { return 0x133; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Iron; } }

        public override short BaseDurability { get { return 192; } }

        public override float BaseArmour { get { return 4; } }

        public override string DisplayName { get { return "Iron Chestplate"; } }
    }

    public class GoldenChestplateItem : ArmourItem
    {
        public static readonly short ItemID = 0x13B;

        public override short ID { get { return 0x13B; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Gold; } }

        public override short BaseDurability { get { return 96; } }

        public override float BaseArmour { get { return 4; } }

        public override string DisplayName { get { return "Golden Chestplate"; } }
    }

    public class DiamondChestplateItem : ArmourItem
    {
        public static readonly short ItemID = 0x137;

        public override short ID { get { return 0x137; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Diamond; } }

        public override short BaseDurability { get { return 384; } }

        public override float BaseArmour { get { return 4; } }

        public override string DisplayName { get { return "Diamond Chestplate"; } }
    }

    public class ChainChestplateItem : ArmourItem
    {
        public static readonly short ItemID = 0x12F;

        public override short ID { get { return 0x12F; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Chain; } }

        public override short BaseDurability { get { return 96; } }

        public override float BaseArmour { get { return 4; } }

        public override string DisplayName { get { return "Chain Chestplate"; } }
    }
}