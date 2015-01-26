using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class SeedsItem : ItemProvider
    {
        public static readonly short ItemID = 0x127;

        public override short ID { get { return 0x127; } }

        public override string DisplayName { get { return "Seeds"; } }
    }
}