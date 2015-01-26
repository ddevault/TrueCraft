using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class BoatItem : ItemProvider
    {
        public static readonly short ItemID = 0x14D;

        public override short ID { get { return 0x14D; } }

        public override sbyte MaximumStack { get { return 1; } }

        public override string DisplayName { get { return "Boat"; } }
    }
}