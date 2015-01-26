using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class GoldenBootsItem : ArmourItem
    {
        public static readonly short ItemID = 0x13D;

        public override short ID { get { return 0x13D; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Gold; } }

        public override short BaseDurability { get { return 80; } }

        public override float BaseArmour { get { return 1.5f; } }

        public override string DisplayName { get { return "Golden Boots"; } }
    }
}