using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class GoldIngotItem : ItemProvider
    {
        public static readonly short ItemID = 0x10A;

        public override short ID { get { return 0x10A; } }

        public override sbyte MaximumStack { get { return 64; } }

        public override string DisplayName { get { return "Gold Ingot"; } }
    }
}