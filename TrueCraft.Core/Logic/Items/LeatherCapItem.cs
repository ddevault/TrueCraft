using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class LeatherCapItem : ArmourItem
    {
        public static readonly short ItemID = 0x12A;

        public override short ID { get { return 0x12A; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Leather; } }

        public override string DisplayName { get { return "Leather Cap"; } }
    }
}