using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class GoldenSwordItem : SwordItem
    {
        public static readonly short ItemID = 0x11B;

        public override short ID { get { return 0x11B; } }

        public override ToolMaterial Material { get { return ToolMaterial.Gold; } }

        public override short BaseDurability { get { return 33; } }

        public override float Damage { get { return 2.5f; } }

        public override string DisplayName { get { return "Golden Sword"; } }
    }
}