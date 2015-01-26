using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class ArrowItem : ItemProvider
    {
        public static readonly short ItemID = 0x106;

        public override short ID { get { return 0x106; } }

        public override string DisplayName { get { return "Arrow"; } }
    }
}