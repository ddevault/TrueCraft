using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueCraft.API
{
    /// <summary>
    /// Enumerates the materials tools can be crafted from.
    /// </summary>
    [Flags]
    public enum ToolMaterial
    {
        /// <summary>
        /// The tool is crafted from no material (special).
        /// </summary>
        None = 1,

        /// <summary>
        /// The tool is crafted from wood.
        /// </summary>
        Wood = 2,

        /// <summary>
        /// The tool is crafted from cobblestone.
        /// </summary>
        Stone = 4,

        /// <summary>
        /// The tool is crafted from iron ingots.
        /// </summary>
        Iron = 8,

        /// <summary>
        /// The tool is crafted from gold ingots.
        /// </summary>
        Gold = 16,

        /// <summary>
        /// The tool is crafted from diamonds.
        /// </summary>
        Diamond = 32,

        /// <summary>
        /// Any tool material is valid in these circumstances.
        /// </summary>
        All = None | Wood | Stone | Iron | Gold | Diamond
    }
}