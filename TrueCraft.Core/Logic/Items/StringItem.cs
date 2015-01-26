using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class StringItem : ItemProvider
    {
        public static readonly short ItemID = 0x11F;

        public override short ID { get { return 0x11F; } }

        public override string DisplayName { get { return "String"; } }
    }
}