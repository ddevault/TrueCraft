using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{

    public abstract class MushroomBlock : BlockProvider
    {
        public override double BlastResistance { get { return 0; } }

        public override double Hardness { get { return 0; } }
    }

    public class BrownMushroomBlock : MushroomBlock
    {
        public static readonly byte BlockID = 0x27;
        
        public override byte ID { get { return 0x27; } }

        public override byte Luminance { get { return 1; } }
        
        public override string DisplayName { get { return "Brown Mushroom"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(13, 1);
        }
    }

    public class RedMushroomBlock : MushroomBlock
    {
        public static readonly byte BlockID = 0x28;

        public override byte ID { get { return 0x28; } }

        public override byte Luminance { get { return 0; } }

        public override string DisplayName { get { return "Red Mushroom"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(12, 1);
        }
    }
}