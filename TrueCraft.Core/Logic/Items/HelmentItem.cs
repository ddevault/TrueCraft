using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class LeatherCapItem : ArmourItem
    {
        public static readonly short ItemID = 0x12A;

        public override short ID { get { return 0x12A; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Leather; } }

        public override short BaseDurability { get { return 34; } }

        public override float BaseArmour { get { return 1.5f; } }

        public override string DisplayName { get { return "Leather Cap"; } }
    }

    public class IronHelmetItem : ArmourItem
    {
        public static readonly short ItemID = 0x132;

        public override short ID { get { return 0x132; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Iron; } }

        public override short BaseDurability { get { return 136; } }

        public override float BaseArmour { get { return 1.5f; } }

        public override string DisplayName { get { return "Iron Helmet"; } }
    }

    public class GoldenHelmetItem : ArmourItem
    {
        public static readonly short ItemID = 0x13A;

        public override short ID { get { return 0x13A; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Gold; } }

        public override short BaseDurability { get { return 68; } }

        public override float BaseArmour { get { return 1.5f; } }

        public override string DisplayName { get { return "Golden Helmet"; } }
    }

    public class DiamondHelmetItem : ArmourItem
    {
        public static readonly short ItemID = 0x136;

        public override short ID { get { return 0x136; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Diamond; } }

        public override short BaseDurability { get { return 272; } }

        public override float BaseArmour { get { return 1.5f; } }

        public override string DisplayName { get { return "Diamond Helmet"; } }
    }

    public class ChainHelmetItem : ArmourItem
    {
        public static readonly short ItemID = 0x12E;

        public override short ID { get { return 0x12E; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Chain; } }

        public override short BaseDurability { get { return 67; } }

        public override float BaseArmour { get { return 1.5f; } }

        public override string DisplayName { get { return "Chain Helmet"; } }
    }
}