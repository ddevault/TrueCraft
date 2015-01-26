using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class StoneSwordItem : ToolItem
    {
        public static readonly short ItemID = 0x110;

        public override short ID { get { return 0x110; } }

        public override ToolMaterial Material { get { return ToolMaterial.Stone; } }

        public override string DisplayName { get { return "Stone Sword"; } }
    }
}