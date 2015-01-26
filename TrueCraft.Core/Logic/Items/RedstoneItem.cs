using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class RedstoneItem : ItemProvider
    {
        public static readonly short ItemID = 0x14B;

        public override short ID { get { return 0x14B; } }

        public override sbyte MaximumStack { get { return 64; } }

        public override string DisplayName { get { return "Redstone"; } }
    }
}