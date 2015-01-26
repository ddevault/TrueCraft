using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
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