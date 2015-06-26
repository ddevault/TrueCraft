using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueCraft.API
{
    /// <summary>
    /// Enumerates the different types of containment between two bounding boxes.
    /// </summary>
    public enum ContainmentType
    {
        /// <summary>
        /// The two bounding boxes are disjoint.
        /// </summary>
        Disjoint,

        /// <summary>
        /// One bounding box contains the other.
        /// </summary>
        Contains,

        /// <summary>
        /// The two bounding boxes intersect.
        /// </summary>
        Intersects
    }

    /// <summary>
    /// Represents an axis-aligned bounding box.
    /// </summary>
    // Mostly taken from the MonoXna project, which is licensed under the MIT license
    public struct BoundingBox : IEquatable<BoundingBox>
    {
        #region Public Fields

        /// <summary>
        /// The minimum vector for the bounding box.
        /// </summary>
        public Vector3 Min;

        /// <summary>
        /// The maximum vector for the bounding box.
        /// </summary>
        public Vector3 Max;

        /// <summary>
        /// The number of corners a bounding box has.
        /// </summary>
        public const int CornerCount = 8;

        #endregion Public Fields


        #region Public Constructors

        /// <summary>
        /// Creates a new bounding box from specified values
        /// </summary>
        /// <param name="min">The minimum vector for the bounding box.</param>
        /// <param name="max">The number of corners a bounding box has.</param>
        public BoundingBox(Vector3 min, Vector3 max)
        {
            this.Min = min;
            this.Max = max;
        }

        /// <summary>
        /// Creates a new bounding box by copying another.
        /// </summary>
        /// <param name="b">The bounding box to clone.</param>
        public BoundingBox(BoundingBox b)
        {
            this.Min = new Vector3(b.Min);
            this.Max = new Vector3(b.Max);
        }

        #endregion Public Constructors


        #region Public Methods

        /// <summary>
        /// Determines the type of containment between this and another bounding box.
        /// </summary>
        /// <param name="box">The other bounding box.</param>
        /// <returns></returns>
        public ContainmentType Contains(BoundingBox box)
        {
            //test if all corner is in the same side of a face by just checking min and max
            if (box.Max.X < Min.X
                || box.Min.X > Max.X
                || box.Max.Y < Min.Y
                || box.Min.Y > Max.Y
                || box.Max.Z < Min.Z
                || box.Min.Z > Max.Z)
                return ContainmentType.Disjoint;


            if (box.Min.X >= Min.X
                && box.Max.X <= Max.X
                && box.Min.Y >= Min.Y
                && box.Max.Y <= Max.Y
                && box.Min.Z >= Min.Z
                && box.Max.Z <= Max.Z)
                return ContainmentType.Contains;

            return ContainmentType.Intersects;
        }

        /// <summary>
        /// Determines whether the specified vector is contained within this bounding box.
        /// </summary>
        /// <param name="vec">The vector.</param>
        /// <returns></returns>
        public bool Contains(Vector3 vec)
        {
            return Min.X <= vec.X && vec.X <= Max.X &&
                Min.Y <= vec.Y && vec.Y <= Max.Y &&
                Min.Z <= vec.Z && vec.Z <= Max.Z;
        }

        /// <summary>
        /// Creates and returns a new bounding box from an enumeration of corner points.
        /// </summary>
        /// <param name="points">The enumeration of corner points.</param>
        /// <returns></returns>
        public static BoundingBox CreateFromPoints(IEnumerable<Vector3> points)
        {
            if (points == null)
                throw new ArgumentNullException();

            bool empty = true;
            Vector3 vector2 = new Vector3(float.MaxValue);
            Vector3 vector1 = new Vector3(float.MinValue);
            foreach (Vector3 vector3 in points)
            {
                vector2 = Vector3.Min(vector2, vector3);
                vector1 = Vector3.Max(vector1, vector3);
                empty = false;
            }
            if (empty)
                throw new ArgumentException();

            return new BoundingBox(vector2, vector1);
        }

        /// <summary>
        /// Offsets this BoundingBox. Does not modify this object, but returns a new one
        /// </summary>
        /// <returns>
        /// The offset bounding box.
        /// </returns>
        /// <param name='Offset'>
        /// The offset.
        /// </param>
        public BoundingBox OffsetBy(Vector3 Offset)
        {
            return new BoundingBox(Min + Offset, Max + Offset);
        }

        /// <summary>
        /// Returns an array of vectors containing the corners of this bounding box.
        /// </summary>
        /// <returns></returns>
        public Vector3[] GetCorners()
        {
            return new Vector3[]
                       {
                           new Vector3(this.Min.X, this.Max.Y, this.Max.Z),
                           new Vector3(this.Max.X, this.Max.Y, this.Max.Z),
                           new Vector3(this.Max.X, this.Min.Y, this.Max.Z),
                           new Vector3(this.Min.X, this.Min.Y, this.Max.Z),
                           new Vector3(this.Min.X, this.Max.Y, this.Min.Z),
                           new Vector3(this.Max.X, this.Max.Y, this.Min.Z),
                           new Vector3(this.Max.X, this.Min.Y, this.Min.Z),
                           new Vector3(this.Min.X, this.Min.Y, this.Min.Z)
                       };
        }

        /// <summary>
        /// Determines whether this and another bounding box are equal.
        /// </summary>
        /// <param name="other">The other bounding box.</param>
        /// <returns></returns>
        public bool Equals(BoundingBox other)
        {
            return (this.Min == other.Min) && (this.Max == other.Max);
        }

        /// <summary>
        /// Determines whether this and another object are equal.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return (obj is BoundingBox) && this.Equals((BoundingBox)obj);
        }

        /// <summary>
        /// Returns the hash code for this bounding box.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Min.GetHashCode() + this.Max.GetHashCode();
        }

        /// <summary>
        /// Determines whether this bounding box intersects another.
        /// </summary>
        /// <param name="box">The other bounding box.</param>
        /// <returns></returns>
        public bool Intersects(BoundingBox box)
        {
            bool result;
            Intersects(ref box, out result);
            return result;
        }

        /// <summary>
        /// Determines whether this bounding box intersects another.
        /// </summary>
        /// <param name="box">The other bounding box.</param>
        /// <param name="result">Set to whether the two bounding boxes intersect.</param>
        public void Intersects(ref BoundingBox box, out bool result)
        {
            if ((this.Max.X > box.Min.X) && (this.Min.X < box.Max.X))
            {
                if ((this.Max.Y < box.Min.Y) || (this.Min.Y > box.Max.Y))
                {
                    result = false;
                    return;
                }

                result = (this.Max.Z > box.Min.Z) && (this.Min.Z < box.Max.Z);
                return;
            }

            result = false;
        }

        public static bool operator ==(BoundingBox a, BoundingBox b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(BoundingBox a, BoundingBox b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// Returns a string representation of this bounding box.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{{Min:{0} Max:{1}}}", this.Min.ToString(), this.Max.ToString());
        }

        #endregion

        /// <summary>
        /// Gets the height of this bounding box.
        /// </summary>
        public double Height
        {
            get { return Max.Y - Min.Y; }
        }

        /// <summary>
        /// Gets the width of this bounding box.
        /// </summary>
        public double Width
        {
            get { return Max.X - Min.X; }
        }

        /// <summary>
        /// Gets the depth of this bounding box.
        /// </summary>
        public double Depth
        {
            get { return Max.Z - Min.Z; }
        }

        /// <summary>
        /// Gets the center of this bounding box.
        /// </summary>
        public Vector3 Center
        {
            get
            {
                return (this.Min + this.Max) / 2;
            }
        }

        public double Volume
        {
            get
            {
                return Width * Height * Depth;
            }
        }
    }
}
