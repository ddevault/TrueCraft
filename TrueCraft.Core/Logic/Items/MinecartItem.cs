using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class MinecartItem : ItemProvider
    {
        public static readonly short ItemID = 0x148;

        public override short ID { get { return 0x148; } }

        public override sbyte MaximumStack { get { return 1; } }

        public override string DisplayName { get { return "Minecart"; } }
    }

    public class MinecartWithChestItem : MinecartItem
    {
        public static readonly short ItemID = 0x156;

        public override short ID { get { return 0x156; } }

        public override string DisplayName { get { return "Minecart with Chest"; } }
    }

    public class MinecartWithFurnaceItem : MinecartItem
    {
        public static readonly short ItemID = 0x157;

        public override short ID { get { return 0x157; } }

        public override string DisplayName { get { return "Minecart with Furnace"; } }
    }
}