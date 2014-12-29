using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class NoteBlockBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x19;
        
        public override byte ID { get { return 0x19; } }

        public override double Hardness { get { return 0.8; } }

        public override string DisplayName { get { return "Note Block"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(10, 4);
        }
    }
}