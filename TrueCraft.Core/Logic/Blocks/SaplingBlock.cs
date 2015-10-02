using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Blocks
{
    public class SaplingBlock : BlockProvider
    {
        public enum SaplingType
        {
            Oak = 0,
            Spruce = 1,
            Birch = 2
        }

        public static readonly byte BlockID = 0x06;
        
        public override byte ID { get { return 0x06; } }
        
        public override double BlastResistance { get { return 0; } }

        public override double Hardness { get { return 0; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }
        
        public override string DisplayName { get { return "Sapling"; } }

        public override BoundingBox? BoundingBox { get { return null; } }

        public override BoundingBox? InteractiveBoundingBox
        {
            get
            {
                return new BoundingBox(new Vector3(1 / 16.0, 0, 1 / 16.0), new Vector3(14 / 16.0));
            }
        }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(15, 0);
        }
    }
}
