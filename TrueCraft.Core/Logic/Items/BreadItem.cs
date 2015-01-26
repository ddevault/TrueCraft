using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class BreadItem : FoodItem
    {
        public static readonly short ItemID = 0x129;

        public override short ID { get { return 0x129; } }

        public override sbyte MaximumStack { get { return 1; } }

        public override float Restores { get { return 2.5f; } }

        public override string DisplayName { get { return "Bread"; } }
    }
}