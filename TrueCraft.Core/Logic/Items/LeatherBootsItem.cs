using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class LeatherBootsItem : ArmourItem
    {
        public static readonly short ItemID = 0x12D;

        public override short ID { get { return 0x12D; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Leather; } }

        public override string DisplayName { get { return "Leather Boots"; } }
    }
}