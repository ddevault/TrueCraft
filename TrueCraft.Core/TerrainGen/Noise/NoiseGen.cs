using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.World;
using TrueCraft.API;
using TrueCraft.Core.World;

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

        public static double BiLinearInterpolate(double X, double Y, double Point00, double Point01, double Point10, double Point11)
        {
            double Point0 = LinearInterpolate(Point00, Point10, X);
            double Point1 = LinearInterpolate(Point01, Point11, X);

            return LinearInterpolate(Point0, Point1, Y);
        }

        public static double TriLinearInterpolate(double X, double Y, double Z, double Point000, double Point001, double Point010, double Point100, double Point011, double Point101, double Point110, double Point111)
        {
            double Point0 = BiLinearInterpolate(X, Y, Point000, Point001, Point100, Point101);
            double Point1 = BiLinearInterpolate(X, Y, Point010, Point011, Point110, Point111);

            return LinearInterpolate(Point0, Point1, Z);
        }
    }

    public enum InterpolateType
    {
        COSINE,
        LINEAR
    }
}