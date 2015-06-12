using System;
using System.Runtime.InteropServices;

namespace TrueCraft.API
{
    /// <summary>
    /// Represents the size of an object in 3D space.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct Size : IEquatable<Size>
    {
        /// <summary>
        /// The width component for the size.
        /// </summary>
        [FieldOffset(0)]
        public double Width;

        /// <summary>
        /// The height component for the size.
        /// </summary>
        [FieldOffset(8)]
        public double Height;

        /// <summary>
        /// The depth component for the size.
        /// </summary>
        [FieldOffset(16)]
        public double Depth;

        #region Constructors

        /// <summary>
        /// Creates a new size from a specified value.
        /// </summary>
        /// <param name="d">The value of the components for the size.</param>
        public Size(double d)
        {
            this.Width = this.Height = this.Depth = d;
        }

        /// <summary>
        /// Creates a new size from specified values.
        /// </summary>
        /// <param name="width">The width component for the size.</param>
        /// <param name="height">The height component for the size.</param>
        /// <param name="depth">The depth component for the size.</param>
        public Size(double width, double height, double depth)
        {
            this.Width = width;
            this.Height = height;
            this.Depth = depth;
        }

        /// <summary>
        /// Creates a new size by copying another.
        /// </summary>
        /// <param name="s">The size to copy.</param>
        public Size(Size s)
        {
            this.Width = s.Width;
            this.Height = s.Height;
            this.Depth = s.Depth;
        }

        #endregion

        #region Operators

        public static Size operator /(Size a, double b)
        {
            return new Size(a.Width / b,
                            a.Height / b,
                            a.Depth / b);
        }

        public static Size operator *(Size a, double b)
        {
            return new Size(a.Width * b,
                            a.Height * b,
                            a.Depth * b);
        }

        public static Size operator %(Size a, double b)
        {
            return new Size(a.Width % b,
                            a.Height % b,
                            a.Depth % b);
        }

        public static Size operator +(Size a, double b)
        {
            return new Size(a.Width + b,
                            a.Height + b,
                            a.Depth + b);
        }

        public static Size operator -(Size a, double b)
        {
            return new Size(a.Width - b,
                            a.Height - b,
                            a.Depth - b);
        }

        public static Size operator /(double a, Size b)
        {
            return new Size(a / b.Width,
                            a / b.Height,
                            a / b.Depth);
        }

        public static Size operator *(double a, Size b)
        {
            return new Size(a * b.Width,
                            a * b.Height,
                            a * b.Depth);
        }

        public static Size operator %(double a, Size b)
        {
            return new Size(a % b.Width,
                            a % b.Height,
                            a % b.Depth);
        }

        public static Size operator +(double a, Size b)
        {
            return new Size(a + b.Width,
                            a + b.Height,
                            a + b.Depth);
        }

        public static Size operator -(double a, Size b)
        {
            return new Size(a - b.Width,
                            a - b.Height,
                            a - b.Depth);
        }

        public static Size operator /(Size a, Size b)
        {
            return new Size(a.Width / b.Width,
                            a.Height / b.Height,
                            a.Depth / b.Depth);
        }

        public static Size operator *(Size a, Size b)
        {
            return new Size(a.Width * b.Width,
                            a.Height * b.Height,
                            a.Depth * b.Depth);
        }

        public static Size operator %(Size a, Size b)
        {
            return new Size(a.Width % b.Width,
                            a.Height % b.Height,
                            a.Depth % b.Depth);
        }

        public static Size operator +(Size a, Size b)
        {
            return new Size(a.Width + b.Width,
                            a.Height + b.Height,
                            a.Depth + b.Depth);
        }

        public static Size operator -(Size a, Size b)
        {
            return new Size(a.Width - b.Width,
                            a.Height - b.Height,
                            a.Depth - b.Depth);
        }

        public static Size operator -(Size a)
        {
            return new Size(-a.Width, -a.Height, -a.Depth);
        }

        public static Size operator +(Size a)
        {
            return new Size(a);
        }

        public static Size operator ++(Size a)
        {
            return new Size(a.Width++,
                            a.Height++,
                            a.Depth++);
        }

        public static Size operator --(Size a)
        {
            return new Size(a.Width--,
                            a.Height--,
                            a.Depth--);
        }

        public static bool operator ==(Size a, Size b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Size a, Size b)
        {
            return !a.Equals(b);
        }

        public static bool operator >(Size a, Size b)
        {
            return a.Volume > b.Volume;
        }

        public static bool operator <(Size a, Size b)
        {
            return a.Volume < b.Volume;
        }

        public static bool operator >=(Size a, Size b)
        {
            return a.Volume >= b.Volume;
        }

        public static bool operator <=(Size a, Size b)
        {
            return a.Volume <= b.Volume;
        }

        #endregion

        #region Conversion operators

        public static implicit operator Size(Vector3 v)
        {
            return new Size(v.X, v.Y, v.Z);
        }

        public static implicit operator Size(Coordinates3D c)
        {
            return new Size(c.X, c.Y, c.Z);
        }


        public static explicit operator Size(Coordinates2D c)
        {
            return new Size(c.X, 0, c.Z);
        }

        public static explicit operator Size(Tuple<double, double, double> t)
        {
            return new Size(t.Item1,
                            t.Item2,
                            t.Item3);
        }

        #endregion

        #region Math methods

        /// <summary>
        /// Returns the component-wise minimum of two sizes.
        /// </summary>
        /// <param name="a">The first size.</param>
        /// <param name="b">The second size.</param>
        /// <returns></returns>
        public static Size Min(Size a, Size b)
        {
            return new Size(Math.Min(a.Width, b.Width),
                            Math.Min(a.Height, b.Height),
                            Math.Min(a.Depth, b.Depth));
        }

        /// <summary>
        /// Returns the component-wise minimum of this and another size.
        /// </summary>
        /// <param name="b">The other size.</param>
        /// <returns></returns>
        public Size Min(Size b)
        {
            return new Size(Math.Min(this.Width, b.Width),
                            Math.Min(this.Height, b.Height),
                            Math.Min(this.Depth, b.Depth));
        }

        /// <summary>
        /// Returns the component-wise maximum of two sizes.
        /// </summary>
        /// <param name="a">The first size.</param>
        /// <param name="b">The second size.</param>
        /// <returns></returns>
        public static Size Max(Size a, Size b)
        {
            return new Size(Math.Max(a.Width, b.Width),
                            Math.Max(a.Height, b.Height),
                            Math.Max(a.Depth, b.Depth));
        }

        /// <summary>
        /// Returns the component-wise maximum of this and another size.
        /// </summary>
        /// <param name="b">The other size.</param>
        /// <returns></returns>
        public Size Max(Size b)
        {
            return new Size(Math.Max(this.Width, b.Width),
                            Math.Max(this.Height, b.Height),
                            Math.Max(this.Depth, b.Depth));
        }

        /// <summary>
        /// Returns the negate of a size.
        /// </summary>
        /// <param name="a">The size to negate.</param>
        /// <returns></returns>
        public static Size Negate(Size a)
        {
            return -a;
        }

        /// <summary>
        /// Returns the negate of this size.
        /// </summary>
        /// <returns></returns>
        public Size Negate()
        {
            return -this;
        }

        /// <summary>
        /// Returns the component-wise absolute value of a size.
        /// </summary>
        /// <param name="a">The size.</param>
        /// <returns></returns>
        public static Size Abs(Size a)
        {
            return new Size(Math.Abs(a.Width),
                            Math.Abs(a.Height),
                            Math.Abs(a.Depth));
        }

        /// <summary>
        /// Returns the component-wise absolute value of this size.
        /// </summary>
        /// <returns></returns>
        public Size Abs()
        {
            return new Size(Math.Abs(this.Width),
                            Math.Abs(this.Height),
                            Math.Abs(this.Depth));
        }

        #endregion

        /// <summary>
        /// Gets the volume of a cuboid with the same dimensions as this size.
        /// </summary>
        public double Volume
        {
            get
            {
                return Width * Height * Depth;
            }
        }

        /// <summary>
        /// Gets the surface area of a cuboid with the same dimensions as this size.
        /// </summary>
        public double SurfaceArea
        {
            get
            {
                return 2 * (Width * Depth +
                            Width * Height +
                            Depth * Height);
            }
        }

        /// <summary>
        /// Gets the lateral surface area of a cuboid with the same dimensions as this size.
        /// </summary>
        public double LateralSurfaceArea
        {
            get
            {
                return 2 * (Width * Depth +
                            Depth * Height);
            }
        }

        /// <summary>
        /// Gets the length of a diagonal line passing through a cuboid with the same dimensions as this size.
        /// </summary>
        public double Diagonal
        {
            get
            {
                return Math.Sqrt(Width * Width +
                                 Height * Height +
                                 Depth * Depth);
            }
        }

        /// <summary>
        /// Returns the average dimension for this size.
        /// </summary>
        public double Average
        {
            get
            {
                return (Width + Height + Depth) / 3;
            }
        }

        /// <summary>
        /// Determines whether this size and another are equal.
        /// </summary>
        /// <param name="other">The other size.</param>
        /// <returns></returns>
        public bool Equals(Size other)
        {
            return this.Width == other.Width &&
                   this.Height == other.Height &&
                   this.Depth == other.Depth;
        }

        /// <summary>
        /// Determines whether this and another object are equal.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is Size && Equals((Size)obj);
        }

        /// <summary>
        /// Returns the hash code for this size.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 449;
                hash = (hash * 457) ^ Width.GetHashCode();
                hash = (hash * 457) ^ Height.GetHashCode();
                hash = (hash * 457) ^ Depth.GetHashCode();
                return hash;
            }
        }
        
        /// <summary>
        /// Returns a string representing the <see cref="Size"/> object in the format of &lt;Width,Height,Depth&gt;.
        /// </summary>
        /// <returns>A string representation of the object</returns>
        /// <inheritdoc cref="Object.ToString"/>
        public override string ToString()
        {
            return string.Format("<{0},{1},{2}>", Width, Height, Depth);
        }
    }
}