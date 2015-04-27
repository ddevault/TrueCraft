using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueCraft.API.World
{
    public interface INoise
    {
        double Value2D(double x, double y);
        double Value3D(double x, double y, double z);
    }
}