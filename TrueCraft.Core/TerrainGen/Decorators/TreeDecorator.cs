using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.World;
using TrueCraft.API;

namespace TrueCraft.Core.TerrainGen.Decorators
{
    public class TreeDecorator : IChunkDecorator
    {
        public void Decorate(IChunk Chunk)
        {
            for (int X = 0; X < 16; X++)
            {
                for (int Z = 0; Z < 16; Z++)
                {

                }
            }
        }

        private void GenerateTree(int X, int Y, int Z, TreeSpecies TreeType)
        {

        }
    }
}
