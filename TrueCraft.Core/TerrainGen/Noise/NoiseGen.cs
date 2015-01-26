using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.World;

namespace TrueCraft.Core.TerrainGen.Noise
{
    public abstract class NoiseGen : INoise
    {
        public abstract double Value2D(double X, double Y);

        public abstract double Value3D(double X, double Y, double Z);

        public int Floor(double Value)
        {
            return (Value >= 0.0 ? (int)Value : (int)Value - 1);
        }
        public static double Interpolate(double Point0, double Point1, double T, InterpolateType Type)
        {
            switch (Type)
            {
                //TODO: Incorporate Cubic Interpolation
                case InterpolateType.COSINE:
                    return CosineInterpolate(Point0, Point1, T);
                case InterpolateType.LINEAR:
                    return LinearInterpolate(Point0, Point1, T);
                default:
                    return LinearInterpolate(Point0, Point1, T);
            }
        }

        private static double CosineInterpolate(double Point0, double Point1, double T)
        {
            var F = T * Math.PI;
            var G = (1 - Math.Cos(F)) * 0.5;
            return Point0 * (1 - G) + Point1 * G;
        }

        //TODO: Implement this into the Interpolate method
        private static double CubicInterpolate(double Point0, double Point1, double Point2, double Point3, double T)
        {
            var E = (Point3 - Point2) - (Point0 - Point1);
            var F = (Point0 - Point1) - E;
            var G = Point2 - Point0;
            var H = Point1;
            return E * Math.Pow(T, 3) + F * Math.Pow(T, 2) + G * T + H;
        }

        private static double LinearInterpolate(double Point0, double Point1, double T)
        {
            return Point0 * (1 - T) + Point1 * T;
        }

        public static double[] ExpandData()
        {
            return new double[0];
        }
    }

    public enum InterpolateType
    {
        COSINE,
        LINEAR
    }
}