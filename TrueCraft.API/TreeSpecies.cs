using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueCraft.API
{
    /// <summary>
    /// Enumerates the different species of trees in TrueCraft.
    /// </summary>
    public enum TreeSpecies
    {
        /// <summary>
        /// An oak tree.
        /// </summary>
        Oak,

        /// <summary>
        /// A birch tree.
        /// </summary>
        Birch,

        /// <summary>
        /// A spruce tree.
        /// </summary>
        Spruce
    }

    /// <summary>
    /// Enumerates the different types of spruce trees in TrueCraft.
    /// </summary>
    /// <remarks>
    /// The following enums are mainly for generation purposes only.
    /// </remarks>
    public enum SpruceType
    {
        //TODO: Spruce types.
    }

    /// <summary>
    /// Enumerates the different types of oak trees in TrueCraft.
    /// </summary>
    public enum OakType
    {
        /// <summary>
        /// Uses layered circles for leaves
        /// </summary>
        Normal,

        /// <summary>
        /// Uses a "blocky" sphere for leaves
        /// </summary>
        BalloonBlocky,

        /// <summary>
        /// Uses a sphere for leaves
        /// </summary>
        Balloon,

        /// <summary>
        /// Uses multiple spheres for leaves and random extra logs acting as branches
        /// </summary>
        Branched
    }
}