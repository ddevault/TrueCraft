using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class StonePickaxeItem : PickaxeItem
    {
        public static readonly short ItemID = 0x112;

        public override short ID { get { return 0x112; } }

        public override ToolMaterial Material { get { return ToolMaterial.Stone; } }

        public override short BaseDurability { get { return 132; } }

        public override string DisplayName { get { return "Stone Pickaxe"; } }
    }
}