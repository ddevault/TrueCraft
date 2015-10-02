using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Blocks
{
    public class RoseBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x26;
        
        public override byte ID { get { return 0x26; } }
        
        public override double BlastResistance { get { return 0; } }

        public override double Hardness { get { return 0; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }
        
        public override string DisplayName { get { return "Rose"; } }

        public override BoundingBox? BoundingBox { get { return null; } }

        public override BoundingBox? InteractiveBoundingBox
        {
            get
            {
                return new BoundingBox(new Vector3(4 / 16.0, 0, 4 / 16.0), new Vector3(12 / 16.0, 8 / 16.0, 12 / 16.0));
            }
        }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(12, 0);
        }
    }
}