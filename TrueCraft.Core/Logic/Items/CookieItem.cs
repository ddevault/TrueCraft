using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class CookieItem : FoodItem
    {
        public static readonly short ItemID = 0x165;

        public override short ID { get { return 0x165; } }

        public override sbyte MaximumStack { get { return 8; } }

        public override float Restores { get { return 0.5f; } }

        public override string DisplayName { get { return "Cookie"; } }
    }
}