using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class BowlItem : ItemProvider
    {
        public static readonly short ItemID = 0x119;

        public override short ID { get { return 0x119; } }

        public override string DisplayName { get { return "Bowl"; } }
    }
}