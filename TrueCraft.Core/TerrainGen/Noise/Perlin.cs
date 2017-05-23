using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueCraft.Core.TerrainGen.Noise
{
    //Perlin Noise implemented using http://freespace.virgin.net/hugo.elias/models/m_perlin.htm
    public class Perlin : NoiseGen
    {
        public int Seed { get; set; }
        public int Octaves { get; set; }
        public double Amplitude { get; set; }
        public double Persistance { get; set; }
        public double Frequency { get; set; }
        public double Lacunarity { get; set; }
        public InterpolateType Interpolation { get; set; }

        public Perlin(int seed)
        {
            this.Seed = seed;
            this.Octaves = 2;
            this.Amplitude = 2;
            this.Persistance = 1;
            this.Frequency = 1;
            this.Lacunarity = 2;
            this.Interpolation = InterpolateType.COSINE;
        }

        /*
         * Psuedo-random number generator methods.
         * For this we use integer noise
         */
        private double Noise2D(double X, double Y)
        {
            var N = ((int)X * 1619 + (int)Y * 31337 * 1013 * Seed) & 0x7fffffff;
            N = (N << 13) ^ N;
            return (1.0 - ((N * (N * N * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0);
        }

        private double Noise3D(double X, double Y, double Z)
        {
            var N = ((int)X * 1619 + (int)Y * 31337 + (int)Z * 52591 * 1013 * Seed) & 0x7fffffff;
            N = (N << 13) ^ N;
            return (1.0 - ((N * (N * N * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0);
        }

        /*
         * Perlin Noise methods
         */
        public override double Value2D(double X, double Y)
        {
            double Total = 0.0;
            double _Frequency = Frequency;
            double _Amplitude = Amplitude;

            for (int I = 0; I < Octaves; I++)
            {
                Total += Interpolated2D(X * _Frequency, Y * _Frequency) * _Amplitude;
                _Frequency *= Lacunarity;
                _Amplitude *= Persistance;
            }
            return Total;
        }

        public override double Value3D(double X, double Y, double Z)
        {
            double Total = 0.0;
            double _Frequency = Frequency;
            double _Amplitude = Amplitude;

            for (int I = 0; I < Octaves; I++)
            {
                Total += Interpolated3D(X * _Frequency, Y * _Frequency, Z * _Frequency) * _Amplitude;
                _Frequency *= Lacunarity;
                _Amplitude *= Persistance;
            }
            return Total;
        }

        /*
         * Smooth Noise Methods
         */
        private double Smooth2D(double X, double Y)
        {
            double X0 = X - 1;
            double X1 = X + 1;
            double Y0 = Y - 1;
            double Y1 = Y + 1;

            double Corners = (Noise2D(X0, Y0) + Noise2D(X1, Y0) + Noise2D(X0, Y1) + Noise2D(X1, Y1)) / 16;
            double Sides = (Noise2D(X0, Y) + Noise2D(X1, Y) + Noise2D(X, Y0) + Noise2D(X, Y1)) / 8;
            double Center = Noise2D(X, Y) / 4;

            return Corners + Sides + Center;
        }

        private double Smooth3D(double X, double Y, double Z)
        {
            double edges = 0;
            edges += Noise3D(X + 1, Y + 1, Z) + Noise3D(X - 1, Y + 1, Z) + Noise3D(X, Y + 1, Z + 1) + Noise3D(X, Y + 1, Z - 1);
            edges += Noise3D(X + 1, Y - 1, Z) + Noise3D(X - 1, Y - 1, Z) + Noise3D(X, Y - 1, Z + 1) + Noise3D(X, Y - 1, Z - 1);
            edges += Noise3D(X + 1, Y, Z + 1) + Noise3D(X + 1, Y, Z - 1) + Noise3D(X - 1, Y, Z + 1) + Noise3D(X - 1, Y, Z - 1);
            edges /= 48;
            double corners = 0;
            corners += Noise3D(X - 1, Y - 1, Z - 1) + Noise3D(X - 1, Y - 1, Z + 1) + Noise3D(X - 1, Y + 1, Z - 1) + Noise3D(X - 1, Y + 1, Z + 1);
            corners += Noise3D(X + 1, Y - 1, Z - 1) + Noise3D(X + 1, Y - 1, Z + 1) + Noise3D(X + 1, Y + 1, Z - 1) + Noise3D(X + 1, Y + 1, Z + 1);
            corners /= 32;
            double sides = 0;
            corners += Noise3D(X - 1, Y, Z) + Noise3D(X - 1, Y, Z) + Noise3D(X, Y + 1, Z);
            corners += Noise3D(X, Y - 1, Z) + Noise3D(X, Y, Z + 1) + Noise3D(X, Y, Z - 1);
            corners /= 16;
            double center = Noise3D(X, Y, Z) / 8;
            return corners + sides + center;
        }

        /*
         * Interpolated Noise Methods
         */
        public double Interpolated2D(double X, double Y)
        {
            //Grid Cell Coordinates
            int X0 = base.Floor(X);
            int X1 = X0 + 1;
            int Y0 = base.Floor(Y);
            int Y1 = Y0 + 1;

            //Interpolation weights
            double SX = X - (double)X0;
            double SY = Y - (double)Y0;

            //Interpolate
            double N0 = Smooth2D(X0, Y0);
            double N1 = Smooth2D(X1, Y0);
            double N2 = Smooth2D(X0, Y1);
            double N3 = Smooth2D(X1, Y1);
            double ix0 = Interpolate(N0, N1, SX, Interpolation);
            double ix1 = Interpolate(N2, N3, SX, Interpolation);
            return Interpolate(ix0, ix1, SY, Interpolation);
        }

        public double Interpolated3D(double X, double Y, double Z)
        {
            //Grid Cell Coordinates
            int X0 = base.Floor(X);
            int X1 = X0 + 1;
            int Y0 = base.Floor(Y);
            int Y1 = Y0 + 1;
            int Z0 = base.Floor(Z);
            int Z1 = Z0 + 1;

            //Interpolation weights
            double SX = X - (double)X0;
            double SY = Y - (double)Y0;
            double SZ = Z - (double)Z0;

            //Interpolate
            double N0 = Smooth3D(X0, Y0, Z0);
            double N1 = Smooth3D(X1, Y0, Z0);
            double N2 = Smooth3D(X0, Y1, Z0);
            double N3 = Smooth3D(X1, Y1, Z0);
            double N4 = Smooth3D(X0, Y0, Z1);
            double N5 = Smooth3D(X1, Y0, Z1);
            double N6 = Smooth3D(X0, Y1, Z1);
            double N7 = Smooth3D(X1, Y1, Z1);
            double ix0 = Interpolate(N0, N1, SX, Interpolation);
            double ix1 = Interpolate(N2, N3, SX, Interpolation);
            double ix2 = Interpolate(N4, N5, SX, Interpolation);
            double ix3 = Interpolate(N6, N7, SX, Interpolation);
            double iy0 = Interpolate(ix0, ix1, SY, Interpolation);
            double iy1 = Interpolate(ix2, ix3, SY, Interpolation);
            return Interpolate(iy0, iy1, SZ, Interpolation);
        }
    }
}