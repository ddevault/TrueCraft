using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class DiamondSwordItem : ToolItem
    {
        public static readonly short ItemID = 0x114;

        public override short ID { get { return 0x114; } }

        public override ToolMaterial Material { get { return ToolMaterial.Diamond; } }

        public override string DisplayName { get { return "Diamond Sword"; } }
    }
}