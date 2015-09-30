using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class ClayItem : ItemProvider
    {
        public static readonly short ItemID = 0x151;

        public override short ID { get { return 0x151; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(9, 3);
        }

        public override string DisplayName { get { return "Clay"; } }
    }
}