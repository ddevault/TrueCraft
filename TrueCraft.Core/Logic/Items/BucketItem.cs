using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public class BucketItem : ItemProvider
    {
        public static readonly short ItemID = 0x145;

        public override short ID { get { return 0x145; } }

        public override sbyte MaximumStack { get { return 1; } }

        public override string DisplayName { get { return "Bucket"; } }
    }

    public class LavabucketItem : BucketItem
    {
        public static readonly new short ItemID = 0x147;

        public override short ID { get { return 0x147; } }

        public override string DisplayName { get { return "Lava bucket"; } }
    }

    public class MilkItem : BucketItem
    {
        public static readonly new short ItemID = 0x14F;

        public override short ID { get { return 0x14F; } }

        public override string DisplayName { get { return "Milk"; } }
    }

    public class WaterBucketItem : BucketItem
    {
        public static readonly new short ItemID = 0x146;

        public override short ID { get { return 0x146; } }

        public override string DisplayName { get { return "Water Bucket"; } }
    }
}