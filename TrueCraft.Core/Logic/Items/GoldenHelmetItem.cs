using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class GoldenHelmetItem : ArmourItem
    {
        public static readonly short ItemID = 0x13A;

        public override short ID { get { return 0x13A; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Gold; } }

        public override short BaseDurability { get { return 68; } }

        public override float BaseArmour { get { return 1.5f; } }

        public override string DisplayName { get { return "Golden Helmet"; } }
    }
}