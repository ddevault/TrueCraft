using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class PaintingItem : ItemProvider
    {
        public static readonly short ItemID = 0x141;

        public override short ID { get { return 0x141; } }

        public override sbyte MaximumStack { get { return 64; } }

        public override string DisplayName { get { return "Painting"; } }
    }
}