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
            return new Size(a/b.Width,
                            a/b.Height,
                            a/b.Depth);
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

        public static bool operator ==(Size a, Size b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Size a, Size b)
        {
            return !(a.Equals(b));
        }

        public static implicit operator Size(Vector3 v)
        {
            return new Size(v.X, v.Y, v.Z);
        }
        #endregion

        // TODO: Create math methods
        
        public bool Equals(Size other)
        {
            return this.Width == other.Width &&
                   this.Height == other.Height &&
                   this.Depth == other.Depth;
        }

        public override bool Equals(object obj)
        {
            if (obj is Size)
                return Equals((Size) obj);
            return false;
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
        public override string ToString()
        {
            return string.Format("<{0},{1},{2}>", Width, Height, Depth);
        }
    }
}