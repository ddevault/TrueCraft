using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class WoodenSwordItem : ToolItem
    {
        public static readonly short ItemID = 0x10C;

        public override short ID { get { return 0x10C; } }

        public override ToolMaterial Material { get { return ToolMaterial.Wood; } }

        public override string DisplayName { get { return "Wooden Sword"; } }
    }
}