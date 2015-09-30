using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class SnowballItem : ItemProvider
    {
        public static readonly short ItemID = 0x14C;

        public override short ID { get { return 0x14C; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(14, 0);
        }

        public override sbyte MaximumStack { get { return 16; } }

        public override string DisplayName { get { return "Snowball"; } }
    }
}