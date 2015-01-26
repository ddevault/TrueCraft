using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.World;

namespace TrueCraft.Core.TerrainGen.Noise
{
    public class ModifyNoise : NoiseGen
    {
        public INoise PrimaryNoise { get; set; }
        public INoise SecondaryNoise { get; set; }
        public NoiseModifier Modifier { get; set; }

        public ModifyNoise(INoise PrimaryNoise, INoise SecondaryNoise, NoiseModifier Modifier = NoiseModifier.Add)
        {
            this.PrimaryNoise = PrimaryNoise;
            this.SecondaryNoise = SecondaryNoise;
            this.Modifier = Modifier;
        }

        public override double Value2D(double X, double Y)
        {
            switch (Modifier)
            {
                case NoiseModifier.Add:
                    return PrimaryNoise.Value2D(X, Y) + SecondaryNoise.Value2D(X, Y);
                case NoiseModifier.Multiply:
                    return PrimaryNoise.Value2D(X, Y) * SecondaryNoise.Value2D(X, Y);
                case NoiseModifier.Power:
                    return Math.Pow(PrimaryNoise.Value2D(X, Y), SecondaryNoise.Value2D(X, Y));
                case NoiseModifier.Subtract:
                    return PrimaryNoise.Value2D(X, Y) - SecondaryNoise.Value2D(X, Y);
                default:
                    //This is unreachable.
                    return PrimaryNoise.Value2D(X, Y) + SecondaryNoise.Value2D(X, Y);
            }
        }

        public override double Value3D(double X, double Y, double Z)
        {
            switch (Modifier)
            {
                case NoiseModifier.Add:
                    return PrimaryNoise.Value3D(X, Y, Z) + SecondaryNoise.Value3D(X, Y, Z);
                case NoiseModifier.Multiply:
                    return PrimaryNoise.Value3D(X, Y, Z) * SecondaryNoise.Value3D(X, Y, Z);
                case NoiseModifier.Power:
                    return Math.Pow(PrimaryNoise.Value3D(X, Y, Z), SecondaryNoise.Value3D(X, Y, Z));
                case NoiseModifier.Subtract:
                    return PrimaryNoise.Value3D(X, Y, Z) - SecondaryNoise.Value3D(X, Y, Z);
                default:
                    //This is unreachable.
                    return PrimaryNoise.Value3D(X, Y, Z) + SecondaryNoise.Value3D(X, Y, Z);
            }
        }
    }

    public enum NoiseModifier
    {
        Add,
        Subtract,
        Multiply,
        Power
    }
}