using System;

namespace TrueCraft.API.Logging
{
    [Flags]
    public enum LogCategory
    {
        /// <summary>
        /// Packets transmitted and received.
        /// </summary>
        Packets = 1,
        /// <summary>
        /// Debug information.
        /// </summary>
        Debug = 2,
        /// <summary>
        /// Potentially harmful, but not catastrophic, problems.
        /// </summary>
        Warning = 4,
        /// <summary>
        /// Catastrophic errors.
        /// </summary>
        Error = 8,
        /// <summary>
        /// Generally useful information.
        /// </summary>
        Notice = 16,
    }
}