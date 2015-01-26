using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class GlowstoneDustItem : ItemProvider
    {
        public static readonly short ItemID = 0x15C;

        public override short ID { get { return 0x15C; } }

        public override string DisplayName { get { return "Glowstone Dust"; } }
    }
}