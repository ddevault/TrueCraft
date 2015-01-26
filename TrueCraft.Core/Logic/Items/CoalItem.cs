using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class CoalItem : ItemProvider
    {
        public static readonly short ItemID = 0x107;

        public override short ID { get { return 0x107; } }

        public override string DisplayName { get { return "Coal"; } }
    }
}