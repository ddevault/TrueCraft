using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class DiamondSwordItem : SwordItem
    {
        public static readonly short ItemID = 0x114;

        public override short ID { get { return 0x114; } }

        public override ToolMaterial Material { get { return ToolMaterial.Diamond; } }

        public override short BaseDurability { get { return 1562; } }

        public override float Damage { get { return 5.5f; } }

        public override string DisplayName { get { return "Diamond Sword"; } }
    }
}