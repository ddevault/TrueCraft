using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class RedstoneRepeaterItem : ItemProvider
    {
        public static readonly short ItemID = 0x164;

        public override short ID { get { return 0x164; } }

        public override sbyte MaximumStack { get { return 64; } }

        public override string DisplayName { get { return "Redstone Repeater"; } }
    }
}