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
        public abstract double Value2D(double x, double y);

        public abstract double Value3D(double x, double y, double z);

        public int Floor(double value)
        {
            return (value >= 0.0 ? (int)value : (int)value - 1);
        }
        public static double Interpolate(double pointA, double pointB, double t, InterpolateType type)
        {
            switch (type)
            {
                //TODO: Incorporate Cubic Interpolation
                case InterpolateType.COSINE:
                    return CosineInterpolate(pointA, pointB, t);
                case InterpolateType.LINEAR:
                    return LinearInterpolate(pointA, pointB, t);
                default:
                    return LinearInterpolate(pointA, pointB, t);
            }
        }

        private static double CosineInterpolate(double pointA, double pointB, double t)
        {
            var F = t * Math.PI;
            var G = (1 - Math.Cos(F)) * 0.5;
            return pointA * (1 - G) + pointB * G;
        }

        //TODO: Implement this into the Interpolate method
        private static double CubicInterpolate(double pointA, double pointB, double pointC, double pointD, double t)
        {
            var E = (pointD - pointC) - (pointA - pointB);
            var F = (pointA - pointB) - E;
            var G = pointC - pointA;
            var H = pointB;
            return E * Math.Pow(t, 3) + F * Math.Pow(t, 2) + G * t + H;
        }

        private static double LinearInterpolate(double pointA, double pointB, double t)
        {
            return pointA * (1 - t) + pointB * t;
        }

        public static double BiLinearInterpolate(double x, double y, double point00, double point01, double point10, double point11)
        {
            double Point0 = LinearInterpolate(point00, point10, x);
            double Point1 = LinearInterpolate(point01, point11, x);

            return LinearInterpolate(Point0, Point1, y);
        }

        public static double TriLinearInterpolate(double x, double y, double z,
            double point000, double point001, double point010,
            double point100, double point011, double point101,
            double point110, double point111)
        {
            double Point0 = BiLinearInterpolate(x, y, point000, point001, point100, point101);
            double Point1 = BiLinearInterpolate(x, y, point010, point011, point110, point111);

            return LinearInterpolate(Point0, Point1, z);
        }
    }

    public enum InterpolateType
    {
        COSINE,
        LINEAR
    }
}