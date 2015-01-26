using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class SugarItem : ItemProvider
    {
        public static readonly short ItemID = 0x161;

        public override short ID { get { return 0x161; } }

        public override string DisplayName { get { return "Sugar"; } }
    }
}