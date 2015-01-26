using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class CompassItem : ToolItem
    {
        public static readonly short ItemID = 0x159;

        public override short ID { get { return 0x159; } }

        public override string DisplayName { get { return "Compass"; } }
    }
}