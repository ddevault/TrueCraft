using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class CookedPorkchopItem : FoodItem
    {
        public static readonly short ItemID = 0x140;

        public override short ID { get { return 0x140; } }

        public override sbyte MaximumStack { get { return 1; } }

        public override float Restores { get { return 4; } }

        public override string DisplayName { get { return "Cooked Porkchop"; } }
    }
}