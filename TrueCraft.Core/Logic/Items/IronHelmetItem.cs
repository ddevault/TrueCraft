using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class IronHelmetItem : ArmourItem
    {
        public static readonly short ItemID = 0x132;

        public override short ID { get { return 0x132; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Iron; } }

        public override short BaseDurability { get { return 136; } }

        public override float BaseArmour { get { return 1.5f; } }

        public override string DisplayName { get { return "Iron Helmet"; } }
    }
}