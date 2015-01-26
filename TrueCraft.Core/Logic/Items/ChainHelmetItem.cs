using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class ChainHelmetItem : ArmourItem
    {
        public static readonly short ItemID = 0x12E;

        public override short ID { get { return 0x12E; } }

        public override ArmourMaterial Material { get { return ArmourMaterial.Chain; } }

        public override string DisplayName { get { return "Chain Helmet"; } }
    }
}