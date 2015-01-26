using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class LeatherItem : ItemProvider
    {
        public static readonly short ItemID = 0x14E;

        public override short ID { get { return 0x14E; } }

        public override string DisplayName { get { return "Leather"; } }
    }
}