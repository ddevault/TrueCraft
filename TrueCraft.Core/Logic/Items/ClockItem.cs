using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class ClockItem : ToolItem
    {
        public static readonly short ItemID = 0x15B;

        public override short ID { get { return 0x15B; } }

        public override string DisplayName { get { return "Clock"; } }
    }
}