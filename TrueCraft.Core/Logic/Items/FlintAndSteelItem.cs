using System;
using TrueCraft.API.Logic;
namespace TrueCraft.Core.Logic.Items
{
    public class FlintAndSteelItem : ToolItem
    {
        public static readonly short ItemID = 0x103;
        public override short ID { get { return 0x103; } }
        public override sbyte MaximumStack { get { return 1; } }
        public override short BaseDurability { get { return 65; } }
        public override string DisplayName { get { return "Flint and Steel"; } }
    }
}
