using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class GoldenChestplateItem : ArmourItem
    {
        public static readonly short ItemID = 0x13B;

        public override short ID { get { return 0x13B; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Gold; } }

        public override string DisplayName { get { return "Golden Chestplate"; } }
    }
}