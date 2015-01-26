using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class DyeItem : ItemProvider
    {
        public static readonly short ItemID = 0x15F;

        public override short ID { get { return 0x15F; } }

        public override string DisplayName { get { return "Dye"; } }
    }
}