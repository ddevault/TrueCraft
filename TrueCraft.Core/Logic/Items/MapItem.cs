using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class MapItem : ToolItem
    {
        public static readonly short ItemID = 0x166;

        public override short ID { get { return 0x166; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(12, 3);
        }

        public override sbyte MaximumStack { get { return 1; } }

        public override string DisplayName { get { return "Map"; } }
    }
}