using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
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