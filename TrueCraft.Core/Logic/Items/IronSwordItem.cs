using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class IronSwordItem : SwordItem
    {
        public static readonly short ItemID = 0x10B;

        public override short ID { get { return 0x10B; } }

        public override ToolMaterial Material { get { return ToolMaterial.Iron; } }

        public override short BaseDurability { get { return 251; } }

        public override float Damage { get { return 4.5f; } }

        public override string DisplayName { get { return "Iron Sword"; } }
    }
}