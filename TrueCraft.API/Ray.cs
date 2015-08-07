using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueCraft.API
{
    /// <summary>
    /// Represents a ray; a line with a start and direction, but no end.
    /// </summary>
    // Mostly taken from the MonoXna project, which is licensed under the MIT license
    public struct Ray : IEquatable<Ray>
    {
        #region Public Fields

        /// <summary>
        /// The direction of the ray.
        /// </summary>
        public readonly Vector3 Direction;

        /// <summary>
        /// The position of the ray (its origin).
        /// </summary>
        public readonly Vector3 Position;

        #endregion


        #region Public Constructors

        /// <summary>
        /// Creates a new ray from specified values.
        /// </summary>
        /// <param name="position">The position of the ray (its origin).</param>
        /// <param name="direction">The direction of the ray.</param>
        public Ray(Vector3 position, Vector3 direction)
        {
            this.Position = position;
            this.Direction = direction;
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Determines whether this and another object are equal.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return (obj is Ray) && Equals((Ray)obj);
        }


        /// <summary>
        /// Determines whether this and another ray are equal.
        /// </summary>
        /// <param name="other">The other ray.</param>
        /// <returns></returns>
        public bool Equals(Ray other)
        {
            return Position.Equals(other.Position) && Direction.Equals(other.Direction);
        }

        /// <summary>
        /// Returns the hash code for this ray.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Position.GetHashCode() ^ Direction.GetHashCode();
        }

        /// <summary>
        /// Returns the distance along the ray where it intersects the specified bounding box, if it intersects at all.
        /// </summary>
        /// <param name="box">The bounding box to check intersection with.</param>
        /// <returns></returns>
        public double? Intersects(BoundingBox box)
        {
            //first test if start in box
            if (Position.X >= box.Min.X
                    && Position.X <= box.Max.X
                    && Position.Y >= box.Min.Y
                    && Position.Y <= box.Max.Y
                    && Position.Z >= box.Min.Z
                    && Position.Z <= box.Max.Z)
                return 0.0f;// here we concidere cube is full and origine is in cube so intersect at origine

            //Second we check each face
            Vector3 maxT = new Vector3(-1.0f);
            //Vector3 minT = new Vector3(-1.0f);
            //calcul intersection with each faces
            if (Direction.X != 0.0f)
            {
                if (Position.X < box.Min.X)
                    maxT.X = (box.Min.X - Position.X) / Direction.X;
                else if (Position.X > box.Max.X)
                    maxT.X = (box.Max.X - Position.X) / Direction.X;
            }

            if (Direction.Y != 0.0f)
            {
                if (Position.Y < box.Min.Y)
                    maxT.Y = (box.Min.Y - Position.Y) / Direction.Y;
                else if (Position.Y > box.Max.Y)
                    maxT.Y = (box.Max.Y - Position.Y) / Direction.Y;
            }

            if (Direction.Z != 0.0f)
            {
                if (Position.Z < box.Min.Z)
                    maxT.Z = (box.Min.Z - Position.Z) / Direction.Z;
                else if (Position.Z > box.Max.Z)
                    maxT.Z = (box.Max.Z - Position.Z) / Direction.Z;
            }

            //get the maximum maxT
            if (maxT.X > maxT.Y && maxT.X > maxT.Z)
            {
                if (maxT.X < 0.0f)
                    return null;// ray go on opposite of face
                //coordonate of hit point of face of cube
                double coord = Position.Z + maxT.X * Direction.Z;
                // if hit point coord ( intersect face with ray) is out of other plane coord it miss
                if (coord < box.Min.Z || coord > box.Max.Z)
                    return null;
                coord = Position.Y + maxT.X * Direction.Y;
                if (coord < box.Min.Y || coord > box.Max.Y)
                    return null;
                return maxT.X;
            }
            if (maxT.Y > maxT.X && maxT.Y > maxT.Z)
            {
                if (maxT.Y < 0.0f)
                    return null;// ray go on opposite of face
                //coordonate of hit point of face of cube
                double coord = Position.Z + maxT.Y * Direction.Z;
                // if hit point coord ( intersect face with ray) is out of other plane coord it miss
                if (coord < box.Min.Z || coord > box.Max.Z)
                    return null;
                coord = Position.X + maxT.Y * Direction.X;
                if (coord < box.Min.X || coord > box.Max.X)
                    return null;
                return maxT.Y;
            }
            else //Z
            {
                if (maxT.Z < 0.0f)
                    return null;// ray go on opposite of face
                //coordonate of hit point of face of cube
                double coord = Position.X + maxT.Z * Direction.X;
                // if hit point coord ( intersect face with ray) is out of other plane coord it miss
                if (coord < box.Min.X || coord > box.Max.X)
                    return null;
                coord = Position.Y + maxT.Z * Direction.Y;
                if (coord < box.Min.Y || coord > box.Max.Y)
                    return null;
                return maxT.Z;
            }
        }

        public static bool operator !=(Ray a, Ray b)
        {
            return !a.Equals(b);
        }

        public static bool operator ==(Ray a, Ray b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Returns a string representation of this ray.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{{Position:{0} Direction:{1}}}", Position.ToString(), Direction.ToString());
        }

        #endregion
    }
}
