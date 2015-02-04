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
        [FieldOffset(0)]
        public double Width;
        [FieldOffset(8)]
        public double Height;
        [FieldOffset(16)]
        public double Depth;

        #region Constructors

        public Size(double d)
        {
            this.Width = this.Height = this.Depth = d;
        }
        public Size(double width, double height, double depth)
        {
            this.Width = width;
            this.Height = height;
            this.Depth = depth;
        }

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

        public static Size Min(Size a, Size b)
        {
            return new Size(Math.Min(a.Width, b.Width),
                            Math.Min(a.Height, b.Height),
                            Math.Min(a.Depth, b.Depth));
        }

        public Size Min(Size b)
        {
            return new Size(Math.Min(this.Width, b.Width),
                            Math.Min(this.Height, b.Height),
                            Math.Min(this.Depth, b.Depth));
        }

        public static Size Max(Size a, Size b)
        {
            return new Size(Math.Max(a.Width, b.Width),
                            Math.Max(a.Height, b.Height),
                            Math.Max(a.Depth, b.Depth));
        }

        public Size Max(Size b)
        {
            return new Size(Math.Max(this.Width, b.Width),
                            Math.Max(this.Height, b.Height),
                            Math.Max(this.Depth, b.Depth));
        }

        public static Size Negate(Size a)
        {
            return -a;
        }

        public Size Negate()
        {
            return -this;
        }

        public static Size Abs(Size a)
        {
            return new Size(Math.Abs(a.Width),
                            Math.Abs(a.Height),
                            Math.Abs(a.Depth));
        }

        public Size Abs()
        {
            return new Size(Math.Abs(this.Width),
                            Math.Abs(this.Height),
                            Math.Abs(this.Depth));
        }

        #endregion

        public double Volume
        {
            get
            {
                return Width * Height * Depth;
            }
        }

        public double SurfaceArea
        {
            get
            {
                return 2 * (Width * Depth +
                            Width * Height +
                            Depth * Height);
            }
        }

        public double LateralSurfaceArea
        {
            get
            {
                return 2 * (Width * Depth +
                            Depth * Height);
            }
        }

        public double Diagonal
        {
            get
            {
                return Math.Sqrt(Width * Width +
                                 Height * Height +
                                 Depth * Depth);
            }
        }

        public double Average
        {
            get
            {
                return (Width + Height + Depth) / 3;
            }
        }

        public bool Equals(Size other)
        {
            return this.Width == other.Width &&
                   this.Height == other.Height &&
                   this.Depth == other.Depth;
        }

        public override bool Equals(object obj)
        {
            return obj is Size && Equals((Size)obj);
        }

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