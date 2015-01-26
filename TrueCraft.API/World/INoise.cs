using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueCraft.API.World
{
    public interface INoise
    {
        double Value2D(double X, double Y);
        double Value3D(double X, double Y, double Z);
    }
}