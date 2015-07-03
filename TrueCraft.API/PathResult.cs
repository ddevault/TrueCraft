using System;
using System.Collections.Generic;

namespace TrueCraft.API
{
    public class PathResult
    {
        public PathResult()
        {
            Index = 0;
        }

        public IList<Coordinates3D> Waypoints;
        public int Index;
    }
}