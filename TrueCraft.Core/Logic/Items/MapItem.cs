using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class MapItem : ItemProvider
    {
        public static readonly short ItemID = 0x166;

        public override short ID { get { return 0x166; } }

        public override sbyte MaximumStack { get { return 1; } }

        public override string DisplayName { get { return "Map"; } }
    }
}