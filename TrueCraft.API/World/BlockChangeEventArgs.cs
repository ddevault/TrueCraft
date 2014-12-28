using System;

namespace TrueCraft.API.World
{
    public class BlockChangeEventArgs : EventArgs
    {
        public BlockChangeEventArgs(Coordinates3D position, BlockData oldBlock, BlockData newBlock)
        {
            Position = position;
            OldBlock = oldBlock;
            NewBlock = newBlock;
        }

        public Coordinates3D Position;
        public BlockData OldBlock;
        public BlockData NewBlock;
    }
}