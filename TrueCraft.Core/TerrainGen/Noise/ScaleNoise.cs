using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueCraft.Core.TerrainGen.Noise
{
    public class ScaledNoise : NoiseGen
    {
        public NoiseGen Noise { get; set; }
        public double Bias { get; set; }
        public double Scale { get; set; }
        public ScaledNoise(NoiseGen Noise)
        {
            this.Noise = Noise;
            Bias = 0;
            Scale = 1;
        }
    
        public override double  Value2D(double X, double Y)
        {
            return Noise.Value2D(X, Y) * Scale + Bias;
        }

        public override double  Value3D(double X, double Y, double Z)
        {
            return Noise.Value3D(X, Y, Z) * Scale + Bias;
        }
    }
}