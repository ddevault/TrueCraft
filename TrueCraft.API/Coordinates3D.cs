using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueCraft.API
{
    public struct Coordinates3D : IEquatable<Coordinates3D>
    {
        public int X, Y, Z;

        public Coordinates3D(int value)
        {
            X = Y = Z = value;
        }

        public Coordinates3D(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Coordinates3D(Coordinates3D v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }

        /// <summary>
        /// Converts this Coordinates3D to a string in the format &lt;x, y, z&gt;.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("<{0},{1},{2}>", X, Y, Z);
        }

        #region Math

        /// <summary>
        /// Clamps the coordinates to within the specified value.
        /// </summary>
        /// <param name="value">Value.</param>
        public void Clamp(int value)
        {
            // TODO: Fix for negative values
            if (Math.Abs(X) > value)
                X = value * (X < 0 ? -1 : 1);
            if (Math.Abs(Y) > value)
                Y = value * (Y < 0 ? -1 : 1);
            if (Math.Abs(Z) > value)
                Z = value * (Z < 0 ? -1 : 1);
        }

        /// <summary>
        /// Calculates the distance between two Coordinates3D objects.
        /// </summary>
        public double DistanceTo(Coordinates3D other)
        {
            return Math.Sqrt(Square(other.X - X) +
                             Square(other.Y - Y) +
                             Square(other.Z - Z));
        }

        /// <summary>
        /// Calculates the square of a num.
        /// </summary>
        private int Square(int num)
        {
            return num * num;
        }

        /// <summary>
        /// Finds the distance of this Coordinate3D from Coordinates3D.Zero
        /// </summary>
        public double Distance
        {
            get
            {
                return DistanceTo(Zero);
            }
        }

        public static Coordinates3D Min(Coordinates3D value1, Coordinates3D value2)
        {
            return new Coordinates3D(
                Math.Min(value1.X, value2.X),
                Math.Min(value1.Y, value2.Y),
                Math.Min(value1.Z, value2.Z)
                );
        }

        public static Coordinates3D Max(Coordinates3D value1, Coordinates3D value2)
        {
            return new Coordinates3D(
                Math.Max(value1.X, value2.X),
                Math.Max(value1.Y, value2.Y),
                Math.Max(value1.Z, value2.Z)
                );
        }

        #endregion

        #region Operators

        public static bool operator !=(Coordinates3D a, Coordinates3D b)
        {
            return !a.Equals(b);
        }

        public static bool operator ==(Coordinates3D a, Coordinates3D b)
        {
            return a.Equals(b);
        }

        public static Coordinates3D operator +(Coordinates3D a, Coordinates3D b)
        {
            return new Coordinates3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Coordinates3D operator -(Coordinates3D a, Coordinates3D b)
        {
            return new Coordinates3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Coordinates3D operator -(Coordinates3D a)
        {
            return new Coordinates3D(-a.X, -a.Y, -a.Z);
        }

        public static Coordinates3D operator *(Coordinates3D a, Coordinates3D b)
        {
            return new Coordinates3D(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        public static Coordinates3D operator /(Coordinates3D a, Coordinates3D b)
        {
            return new Coordinates3D(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

        public static Coordinates3D operator %(Coordinates3D a, Coordinates3D b)
        {
            return new Coordinates3D(a.X % b.X, a.Y % b.Y, a.Z % b.Z);
        }

        public static Coordinates3D operator +(Coordinates3D a, int b)
        {
            return new Coordinates3D(a.X + b, a.Y + b, a.Z + b);
        }

        public static Coordinates3D operator -(Coordinates3D a, int b)
        {
            return new Coordinates3D(a.X - b, a.Y - b, a.Z - b);
        }

        public static Coordinates3D operator *(Coordinates3D a, int b)
        {
            return new Coordinates3D(a.X * b, a.Y * b, a.Z * b);
        }

        public static Coordinates3D operator /(Coordinates3D a, int b)
        {
            return new Coordinates3D(a.X / b, a.Y / b, a.Z / b);
        }

        public static Coordinates3D operator %(Coordinates3D a, int b)
        {
            return new Coordinates3D(a.X % b, a.Y % b, a.Z % b);
        }

        public static Coordinates3D operator +(int a, Coordinates3D b)
        {
            return new Coordinates3D(a + b.X, a + b.Y, a + b.Z);
        }

        public static Coordinates3D operator -(int a, Coordinates3D b)
        {
            return new Coordinates3D(a - b.X, a - b.Y, a - b.Z);
        }

        public static Coordinates3D operator *(int a, Coordinates3D b)
        {
            return new Coordinates3D(a * b.X, a * b.Y, a * b.Z);
        }

        public static Coordinates3D operator /(int a, Coordinates3D b)
        {
            return new Coordinates3D(a / b.X, a / b.Y, a / b.Z);
        }

        public static Coordinates3D operator %(int a, Coordinates3D b)
        {
            return new Coordinates3D(a % b.X, a % b.Y, a % b.Z);
        }

        #endregion

        #region Conversion operators

        public static explicit operator Coordinates3D(Coordinates2D a)
        {
            return new Coordinates3D(a.X, 0, a.Z);
        }

        public static explicit operator Coordinates3D(Vector3 a)
        {
            return new Coordinates3D((int)a.X,
                                     (int)a.Y,
                                     (int)a.Z);
        }

        public static explicit operator Coordinates3D(Size s)
        {
            return new Coordinates3D((int)s.Width,
                                     (int)s.Height,
                                     (int)s.Depth);
        }

        #endregion

        #region Constants

        public static readonly Coordinates3D Zero = new Coordinates3D(0);
        public static readonly Coordinates3D One = new Coordinates3D(1);

        public static readonly Coordinates3D Up = new Coordinates3D(0, 1, 0);
        public static readonly Coordinates3D Down = new Coordinates3D(0, -1, 0);
        public static readonly Coordinates3D Left = new Coordinates3D(-1, 0, 0);
        public static readonly Coordinates3D Right = new Coordinates3D(1, 0, 0);
        public static readonly Coordinates3D Backwards = new Coordinates3D(0, 0, -1);
        public static readonly Coordinates3D Forwards = new Coordinates3D(0, 0, 1);

        public static readonly Coordinates3D East = new Coordinates3D(1, 0, 0);
        public static readonly Coordinates3D West = new Coordinates3D(-1, 0, 0);
        public static readonly Coordinates3D North = new Coordinates3D(0, 0, -1);
        public static readonly Coordinates3D South = new Coordinates3D(0, 0, 1);

        public static readonly Coordinates3D OneX = new Coordinates3D(1, 0, 0);
        public static readonly Coordinates3D OneY = new Coordinates3D(0, 1, 0);
        public static readonly Coordinates3D OneZ = new Coordinates3D(0, 0, 1);

        #endregion

        public bool Equals(Coordinates3D other)
        {
            return other.X.Equals(X) && other.Y.Equals(Y) && other.Z.Equals(Z);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(Coordinates3D)) return false;
            return Equals((Coordinates3D)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = X.GetHashCode();
                result = (result * 397) ^ Y.GetHashCode();
                result = (result * 397) ^ Z.GetHashCode();
                return result;
            }
        }
    }
}
