using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class DiamondLeggingsItem : ArmourItem
    {
        public static readonly short ItemID = 0x138;

        public override short ID { get { return 0x138; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Diamond; } }

        public override short BaseDurability { get { return 368; } }

        public override float BaseArmour { get { return 3; } }

        public override string DisplayName { get { return "Diamond Leggings"; } }
    }
}