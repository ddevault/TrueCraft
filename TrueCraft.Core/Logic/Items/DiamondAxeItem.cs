using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class DiamondAxeItem : ToolItem
    {
        public static readonly short ItemID = 0x117;

        public override short ID { get { return 0x117; } }

        public override ToolMaterial Material { get { return ToolMaterial.Diamond; } }

        public override string DisplayName { get { return "Diamond Axe"; } }
    }
}