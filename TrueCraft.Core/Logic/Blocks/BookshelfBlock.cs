using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class BookshelfBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x2F;
        
        public override byte ID { get { return 0x2F; } }

        public override double Hardness { get { return 1.5; } }

        public override string DisplayName { get { return "Bookshelf"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(3, 2);
        }
    }
}