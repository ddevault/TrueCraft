using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class CakeItem : FoodItem
    {
        public static readonly short ItemID = 0x162;

        public override short ID { get { return 0x162; } }

        public override sbyte MaximumStack { get { return 1; } }

        public override float Restores { get { return 1.5f; } }

        public override string DisplayName { get { return "Cake"; } }
    }
}