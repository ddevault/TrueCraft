using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.World;

namespace TrueCraft.Core.TerrainGen.Noise
{
    public class InvertNoise : NoiseGen
    {
        public INoise Noise { get; set; }
        public InvertNoise(INoise Noise)
        {
            this.Noise = Noise;
        }
    
        public override double  Value2D(double X, double Y)
        {
            return -Noise.Value2D(X, Y);
        }

        public override double  Value3D(double X, double Y, double Z)
        {
            return -Noise.Value3D(X, Y, Z);
        }
    }
}