using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class ShearsItem : ToolItem
    {
        public static readonly short ItemID = 0x167;

        public override short ID { get { return 0x167; } }

        public override sbyte MaximumStack { get { return 1; } }

        public override short BaseDurability { get { return 239; } }

        public override string DisplayName { get { return "Shears"; } }
    }
}