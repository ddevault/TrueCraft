using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class SugarCanesItem : ItemProvider
    {
        public static readonly short ItemID = 0x152;

        public override short ID { get { return 0x152; } }

        public override string DisplayName { get { return "Sugar Canes"; } }
    }
}