using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class SignItem : ItemProvider
    {
        public static readonly short ItemID = 0x143;

        public override short ID { get { return 0x143; } }

        public override sbyte MaximumStack { get { return 1; } }

        public override string DisplayName { get { return "Sign"; } }
    }
}