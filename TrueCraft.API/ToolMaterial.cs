using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueCraft.API
{
    /// <summary>
    /// Enumerates the materials tools can be crafted from.
    /// </summary>
    public enum ToolMaterial
    {
        /// <summary>
        /// The tool is crafted from no material (special).
        /// </summary>
        None,

        /// <summary>
        /// The tool is crafted from wood.
        /// </summary>
        Wood,

        /// <summary>
        /// The tool is crafted from cobblestone.
        /// </summary>
        Stone,

        /// <summary>
        /// The tool is crafted from iron ingots.
        /// </summary>
        Iron,

        /// <summary>
        /// The tool is crafted from gold ingots.
        /// </summary>
        Gold,

        /// <summary>
        /// The tool is crafted from diamonds.
        /// </summary>
        Diamond
    }
}