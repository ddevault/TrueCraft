using System;
using TrueCraft.API.Logic;
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
}