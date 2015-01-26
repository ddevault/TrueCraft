using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class IronHoeItem : HoeItem
    {
        public static readonly short ItemID = 0x124;

        public override short ID { get { return 0x124; } }

        public override ToolMaterial Material { get { return ToolMaterial.Iron; } }

        public override short BaseDurability { get { return 251; } }

        public override string DisplayName { get { return "Iron Hoe"; } }
    }
}