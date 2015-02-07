using System;

namespace TrueCraft.API
{
    public struct Coordinates2D : IEquatable<Coordinates2D>
    {
        public int X, Z;

        public Coordinates2D(int value)
        {
            X = Z = value;
        }

        public Coordinates2D(int x, int z)
        {
            X = x;
            Z = z;
        }

        public Coordinates2D(Coordinates2D v)
        {
            X = v.X;
            Z = v.Z;
        }

        /// <summary>
        /// Converts this Coordinates2D to a string in the format &lt;x,z&gt;.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("<{0},{1}>", X, Z);
        }

        #region Math

        /// <summary>
        /// Calculates the distance between two Coordinates2D objects.
        /// </summary>
        public double DistanceTo(Coordinates2D other)
        {
            return Math.Sqrt(Square(other.X - X) +
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
        /// Finds the distance of this Coordinates2D from Coordinates2D.Zero
        /// </summary>
        public double Distance
        {
            get
            {
                return DistanceTo(Zero);
            }
        }

        public static Coordinates2D Min(Coordinates2D value1, Coordinates2D value2)
        {
            return new Coordinates2D(
                Math.Min(value1.X, value2.X),
                Math.Min(value1.Z, value2.Z)
                );
        }

        public static Coordinates2D Max(Coordinates2D value1, Coordinates2D value2)
        {
            return new Coordinates2D(
                Math.Max(value1.X, value2.X),
                Math.Max(value1.Z, value2.Z)
                );
        }

        #endregion

        #region Operators

        public static bool operator !=(Coordinates2D a, Coordinates2D b)
        {
            return !a.Equals(b);
        }

        public static bool operator ==(Coordinates2D a, Coordinates2D b)
        {
            return a.Equals(b);
        }

        public static Coordinates2D operator +(Coordinates2D a, Coordinates2D b)
        {
            return new Coordinates2D(a.X + b.X, a.Z + b.Z);
        }

        public static Coordinates2D operator -(Coordinates2D a, Coordinates2D b)
        {
            return new Coordinates2D(a.X - b.X, a.Z - b.Z);
        }

        public static Coordinates2D operator -(Coordinates2D a)
        {
            return new Coordinates2D(
                -a.X,
                -a.Z);
        }

        public static Coordinates2D operator *(Coordinates2D a, Coordinates2D b)
        {
            return new Coordinates2D(a.X * b.X, a.Z * b.Z);
        }

        public static Coordinates2D operator /(Coordinates2D a, Coordinates2D b)
        {
            return new Coordinates2D(a.X / b.X, a.Z / b.Z);
        }

        public static Coordinates2D operator %(Coordinates2D a, Coordinates2D b)
        {
            return new Coordinates2D(a.X % b.X, a.Z % b.Z);
        }

        public static Coordinates2D operator +(Coordinates2D a, int b)
        {
            return new Coordinates2D(a.X + b, a.Z + b);
        }

        public static Coordinates2D operator -(Coordinates2D a, int b)
        {
            return new Coordinates2D(a.X - b, a.Z - b);
        }

        public static Coordinates2D operator *(Coordinates2D a, int b)
        {
            return new Coordinates2D(a.X * b, a.Z * b);
        }

        public static Coordinates2D operator /(Coordinates2D a, int b)
        {
            return new Coordinates2D(a.X / b, a.Z / b);
        }

        public static Coordinates2D operator %(Coordinates2D a, int b)
        {
            return new Coordinates2D(a.X % b, a.Z % b);
        }

        public static Coordinates2D operator +(int a, Coordinates2D b)
        {
            return new Coordinates2D(a + b.X, a + b.Z);
        }

        public static Coordinates2D operator -(int a, Coordinates2D b)
        {
            return new Coordinates2D(a - b.X, a - b.Z);
        }

        public static Coordinates2D operator *(int a, Coordinates2D b)
        {
            return new Coordinates2D(a * b.X, a * b.Z);
        }

        public static Coordinates2D operator /(int a, Coordinates2D b)
        {
            return new Coordinates2D(a / b.X, a / b.Z);
        }

        public static Coordinates2D operator %(int a, Coordinates2D b)
        {
            return new Coordinates2D(a % b.X, a % b.Z);
        }

        public static explicit operator Coordinates2D(Coordinates3D a)
        {
            return new Coordinates2D(a.X, a.Z);
        }

        #endregion

        #region Constants

        public static readonly Coordinates2D Zero = new Coordinates2D(0);
        public static readonly Coordinates2D One = new Coordinates2D(1);

        public static readonly Coordinates2D Forward = new Coordinates2D(0, 1);
        public static readonly Coordinates2D Backward = new Coordinates2D(0, -1);
        public static readonly Coordinates2D Left = new Coordinates2D(-1, 0);
        public static readonly Coordinates2D Right = new Coordinates2D(1, 0);

        #endregion

        public bool Equals(Coordinates2D other)
        {
            return other.X.Equals(X) && other.Z.Equals(Z);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(Coordinates2D)) return false;
            return Equals((Coordinates2D)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = X.GetHashCode();
                result = (result * 397) ^ Z.GetHashCode();
                return result;
            }
        }
    }
}