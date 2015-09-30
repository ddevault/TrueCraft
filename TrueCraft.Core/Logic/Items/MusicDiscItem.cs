using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class MusicDiscItem : ItemProvider
    {
        public static readonly short ItemID = 0x8D1;

        public override short ID { get { return 0x8D1; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(1, 15);
        }

        public override sbyte MaximumStack { get { return 1; } }

        public override string DisplayName { get { return "Music Disc"; } }
    }
}