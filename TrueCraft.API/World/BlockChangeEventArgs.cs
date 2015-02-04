using System;
using TrueCraft.API.Logic;

namespace TrueCraft.API.World
{
    public class BlockChangeEventArgs : EventArgs
    {
        public BlockChangeEventArgs(Coordinates3D position, BlockDescriptor oldBlock, BlockDescriptor newBlock)
        {
            Position = position;
            OldBlock = oldBlock;
            NewBlock = newBlock;
        }

        public Coordinates3D Position;
        public BlockDescriptor OldBlock;
        public BlockDescriptor NewBlock;
    }
}