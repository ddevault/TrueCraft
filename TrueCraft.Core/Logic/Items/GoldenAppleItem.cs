using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class GoldenAppleItem : FoodItem
    {
        public static readonly short ItemID = 0x142;

        public override short ID { get { return 0x142; } }

        public override sbyte MaximumStack { get { return 1; } }

        public override float Restores { get { return 10; } }

        public override string DisplayName { get { return "Golden Apple"; } }
    }
}