using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueCraft.Core.Logic.Items
{
    public abstract class DoorItem : ItemProvider
    {
        public override sbyte MaximumStack { get { return 1; } }
    }

    public class IronDoorItem : DoorItem
    {
        public static readonly short ItemID = 0x14A;

        public override short ID { get { return 0x14A; } }

        public override string DisplayName { get { return "Iron Door"; } }
    }

    public class WoodenDoorItem : DoorItem
    {
        public static readonly short ItemID = 0x144;

        public override short ID { get { return 0x144; } }

        public override string DisplayName { get { return "Wooden Door"; } }
    }
}