using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class IronIngotItem : ItemProvider
    {
        public static readonly short ItemID = 0x109;

        public override short ID { get { return 0x109; } }

        public override string DisplayName { get { return "Iron Ingot"; } }
    }
}