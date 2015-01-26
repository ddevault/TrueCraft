using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class MushroomStewItem : FoodItem
    {
        public static readonly short ItemID = 0x11A;

        public override short ID { get { return 0x11A; } }

        public override sbyte MaximumStack { get { return 1; } }

        public override float Restores { get { return 5; } }

        public override string DisplayName { get { return "Mushroom Stew"; } }
    }
}