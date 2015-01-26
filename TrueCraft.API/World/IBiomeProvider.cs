using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.Logic;

namespace TrueCraft.API.World
{
    public interface IBiomeProvider
    {
        byte ID { get; }
        float Temperature { get; }
        byte SurfaceBlock { get; }
        byte FillerBlock { get; }
    }
}
