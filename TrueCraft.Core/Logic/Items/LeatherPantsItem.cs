using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class LeatherPantsItem : ArmourItem
    {
        public static readonly short ItemID = 0x12C;

        public override short ID { get { return 0x12C; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Leather; } }

        public override string DisplayName { get { return "Leather Pants"; } }
    }
}