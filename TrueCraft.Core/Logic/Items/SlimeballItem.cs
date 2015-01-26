using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class SlimeballItem : ItemProvider
    {
        public static readonly short ItemID = 0x155;

        public override short ID { get { return 0x155; } }

        public override string DisplayName { get { return "Slimeball"; } }
    }
}