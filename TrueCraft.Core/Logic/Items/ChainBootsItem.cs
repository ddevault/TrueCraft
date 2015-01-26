using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class ChainBootsItem : ArmourItem
    {
        public static readonly short ItemID = 0x131;

        public override short ID { get { return 0x131; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Chain; } }

        public override string DisplayName { get { return "Chain Boots"; } }
    }
}