using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class GoldenSwordItem : ToolItem
    {
        public static readonly short ItemID = 0x11B;

        public override short ID { get { return 0x11B; } }

        public override ToolMaterial Material { get { return ToolMaterial.Gold; } }

        public override string DisplayName { get { return "Golden Sword"; } }
    }
}