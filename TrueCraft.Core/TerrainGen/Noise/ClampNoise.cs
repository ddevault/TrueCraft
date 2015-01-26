using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.World;

namespace TrueCraft.Core.TerrainGen.Noise
{
    public class ClampNoise : NoiseGen
    {
        public INoise Noise { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public ClampNoise(INoise Noise)
        {
            this.Noise = Noise;
            MinValue = 0;
            MaxValue = 1;
        }
    
        public override double  Value2D(double X, double Y)
        {
            var NoiseValue = Noise.Value2D(X, Y);
            if (NoiseValue < MinValue)
                NoiseValue = MinValue;
            if (NoiseValue > MaxValue)
                NoiseValue = MaxValue;
            return NoiseValue;
        }

        public override double  Value3D(double X, double Y, double Z)
        {
            var NoiseValue = Noise.Value3D(X, Y, Z);
            if (NoiseValue < MinValue)
                NoiseValue = MinValue;
            if (NoiseValue > MaxValue)
                NoiseValue = MaxValue;
            return NoiseValue;
        }
    }
}