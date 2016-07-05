using System;
namespace TrueCraft.API
{
    public struct BoundingCylinder : IEquatable<BoundingCylinder>
    {
        public Vector3 Min;

        public Vector3 Max;

        public double Radius;
    
        public BoundingCylinder(Vector3 min, Vector3 max, double radius)
        {
            Min = min;
            Max = max;
            Radius = radius;
        }

        public bool Intersects(Vector3 q)
        {
            return DistancePointLine(q, Min, Max) < Radius;
        }
        
        public bool Intersects(BoundingBox q)
        {
            var corners = q.GetCorners();
            for (int i = 0; i < corners.Length; i++)
            {
                if (Intersects(corners[i]))
                    return true;
            }
            return false;
        }
        
        // http://answers.unity3d.com/questions/62644/distance-between-a-ray-and-a-point.html
        public static double DistancePointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            return (ProjectPointLine(point, lineStart, lineEnd) - point).Distance;
        }
         
        public static Vector3 ProjectPointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            var rhs = point - lineStart;
            var vector2 = lineEnd - lineStart;
            var magnitude = vector2.Distance;
            var lhs = vector2;
            if (magnitude > 1E-06f)
                lhs = lhs / magnitude;
            var num2 = Vector3.Dot(lhs, rhs);
            if (num2 < 0) num2 = 0;
            if (num2 > magnitude) num2 = magnitude;
            return lineStart + (lhs * num2);
        }

        public bool Equals(BoundingCylinder other)
        {
            return other.Max == this.Max
                && other.Min == this.Min
                && other.Radius == this.Radius;
        }
    }
}