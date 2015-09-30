using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class SaddleItem : ItemProvider
    {
        public static readonly short ItemID = 0x149;

        public override short ID { get { return 0x149; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(8, 6);
        }

        public override sbyte MaximumStack { get { return 1; } }

        public override string DisplayName { get { return "Saddle"; } }
    }
}