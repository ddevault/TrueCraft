using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class GoldenLeggingsItem : ArmourItem
    {
        public static readonly short ItemID = 0x13C;

        public override short ID { get { return 0x13C; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Gold; } }

        public override string DisplayName { get { return "Golden Leggings"; } }
    }
}