using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class CookedFishItem : FoodItem
    {
        public static readonly short ItemID = 0x15E;

        public override short ID { get { return 0x15E; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(10, 5);
        }

        public override float Restores { get { return 2.5f; } }

        public override string DisplayName { get { return "Cooked Fish"; } }
    }
}