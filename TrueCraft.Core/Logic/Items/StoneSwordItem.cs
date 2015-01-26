using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class StoneSwordItem : SwordItem
    {
        public static readonly short ItemID = 0x110;

        public override short ID { get { return 0x110; } }

        public override ToolMaterial Material { get { return ToolMaterial.Stone; } }

        public override short BaseDurability { get { return 132; } }

        public override float Damage { get { return 3.5f; } }

        public override string DisplayName { get { return "Stone Sword"; } }
    }
}