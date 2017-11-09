using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueCraft.API.World
{
    public interface IDecoration
    {
        bool ValidLocation(Coordinates3D location);
        bool GenerateAt(IWorldSeed world, IChunk chunk, Coordinates3D location);
    }
}