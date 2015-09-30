using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class GlowstoneDustItem : ItemProvider
    {
        public static readonly short ItemID = 0x15C;

        public override short ID { get { return 0x15C; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(9, 4);
        }

        public override string DisplayName { get { return "Glowstone Dust"; } }
    }
}