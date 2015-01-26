using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class WoodenDoorItem : ItemProvider
    {
        public static readonly short ItemID = 0x144;

        public override short ID { get { return 0x144; } }

        public override sbyte MaximumStack { get { return 1; } }

        public override string DisplayName { get { return "Wooden Door"; } }
    }
}