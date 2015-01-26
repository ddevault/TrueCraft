using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class DiamondHelmetItem : ArmourItem
    {
        public static readonly short ItemID = 0x136;

        public override short ID { get { return 0x136; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Diamond; } }

        public override string DisplayName { get { return "Diamond Helmet"; } }
    }
}