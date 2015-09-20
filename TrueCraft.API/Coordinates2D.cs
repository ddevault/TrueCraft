using System;

namespace TrueCraft.API
{
    /// <summary>
    /// Represents a tuple of 2D coordinates.
    /// </summary>
    public struct Coordinates2D : IEquatable<Coordinates2D>
    {
        /// <summary>
        /// The X component of the coordinates.
        /// </summary>
        public int X;

        /// <summary>
        /// The Y component of the coordinates.
        /// </summary>
        public int Z;

        /// <summary>
        /// Creates a new pair of coordinates from the specified value.
        /// </summary>
        /// <param name="value">The value for the components of the coordinates.</param>
        public Coordinates2D(int value)
        {
            X = Z = value;
        }

        /// <summary>
        /// Creates a new pair of coordinates from the specified values.
        /// </summary>
        /// <param name="x">The X component of the coordinates.</param>
        /// <param name="z">The Y component of the coordinates.</param>
        public Coordinates2D(int x, int z)
        {
            X = x;
            Z = z;
        }

        /// <summary>
        /// Creates a new pair of coordinates by copying another.
        /// </summary>
        /// <param name="v">The coordinates to copy.</param>
        public Coordinates2D(Coordinates2D v)
        {
            X = v.X;
            Z = v.Z;
        }

        /// <summary>
        /// Returns the string representation of this 2D coordinates.
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

        /// <summary>
        /// Returns the component-wise minimum of two 2D coordinates.
        /// </summary>
        /// <param name="value1">The first coordinates.</param>
        /// <param name="value2">The second coordinates.</param>
        /// <returns></returns>
        public static Coordinates2D Min(Coordinates2D value1, Coordinates2D value2)
        {
            return new Coordinates2D(
                Math.Min(value1.X, value2.X),
                Math.Min(value1.Z, value2.Z)
                );
        }

        /// <summary>
        /// Returns the component-wise maximum of two 2D coordinates.
        /// </summary>
        /// <param name="value1">The first coordinates.</param>
        /// <param name="value2">The second coordinates.</param>
        /// <returns></returns>
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

        /// <summary>
        /// A pair of 2D coordinates with components set to 0.0.
        /// </summary>
        public static readonly Coordinates2D Zero = new Coordinates2D(0);

        /// <summary>
        /// A pair of 2D coordinates with components set to 1.0.
        /// </summary>
        public static readonly Coordinates2D One = new Coordinates2D(1);


        /// <summary>
        /// A pair of 2D coordinates facing forwards.
        /// </summary>
        public static readonly Coordinates2D Forward = new Coordinates2D(0, 1);

        /// <summary>
        /// A pair of 2D coordinates facing backwards.
        /// </summary>
        public static readonly Coordinates2D Backward = new Coordinates2D(0, -1);

        /// <summary>
        /// A pair of 2D coordinates facing left.
        /// </summary>
        public static readonly Coordinates2D Left = new Coordinates2D(-1, 0);

        /// <summary>
        /// A pair of 2D coordinates facing right.
        /// </summary>
        public static readonly Coordinates2D Right = new Coordinates2D(1, 0);

        /// <summary>
        /// A trio of 3D coordinates facing to the east.
        /// </summary>
        public static readonly Coordinates2D East = new Coordinates2D(1, 0);

        /// <summary>
        /// A trio of 3D coordinates facing to the west.
        /// </summary>
        public static readonly Coordinates2D West = new Coordinates2D(-1, 0);

        /// <summary>
        /// A trio of 3D coordinates facing to the north.
        /// </summary>
        public static readonly Coordinates2D North = new Coordinates2D(0, -1);

        /// <summary>
        /// A trio of 3D coordinates facing to the south.
        /// </summary>
        public static readonly Coordinates2D South = new Coordinates2D(0, 1);

        #endregion

        /// <summary>
        /// Determines whether this 2D coordinates and another are equal.
        /// </summary>
        /// <param name="other">The other coordinates.</param>
        /// <returns></returns>
        public bool Equals(Coordinates2D other)
        {
            return other.X.Equals(X) && other.Z.Equals(Z);
        }

        /// <summary>
        /// Determines whether this and another object are equal.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(Coordinates2D)) return false;
            return Equals((Coordinates2D)obj);
        }

        /// <summary>
        /// Returns the hash code for this 2D coordinates.
        /// </summary>
        /// <returns></returns>
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