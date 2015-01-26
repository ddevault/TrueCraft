using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class IronLeggingsItem : ArmourItem
    {
        public static readonly short ItemID = 0x134;

        public override short ID { get { return 0x134; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Iron; } }

        public override short BaseDurability { get { return 184; } }

        public override float BaseArmour { get { return 3; } }

        public override string DisplayName { get { return "Iron Leggings"; } }
    }
}