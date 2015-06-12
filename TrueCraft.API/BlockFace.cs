using System;

namespace TrueCraft.API
{
    /// <summary>
    /// Enumerates the directions of block faces.
    /// </summary>
    public enum BlockFace
    {
        /// <summary>
        /// The block face points towards -Y.
        /// </summary>
        NegativeY = 0,

        /// <summary>
        /// The block face points towards +Y.
        /// </summary>
        PositiveY = 1,

        /// <summary>
        /// The block face points towards -Z.
        /// </summary>
        NegativeZ = 2,

        /// <summary>
        /// The block face points towards +Z.
        /// </summary>
        PositiveZ = 3,

        /// <summary>
        /// The block face points towards -X.
        /// </summary>
        NegativeX = 4,

        /// <summary>
        /// The block face points towards +X.
        /// </summary>
        PositiveX = 5
    }
}