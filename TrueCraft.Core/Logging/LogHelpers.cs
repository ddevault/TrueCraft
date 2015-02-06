using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.Logging;

namespace TrueCraft.Core.Logging
{
    public static class LogHelpers
    {
        public static string GetTimestamp(string format = "u", bool utc = true, string trailingSeparator = "\t")
        {
            return (utc ? DateTime.UtcNow : DateTime.Now).ToString(format) + trailingSeparator;
        }
        public static ConsoleColor GetCategoryColor(LogCategory category)
        {
            switch (category)
            {
                case LogCategory.Packets:
                    return ConsoleColor.Green;
                case LogCategory.Debug:
                    return ConsoleColor.Blue;
                case LogCategory.Warning:
                    return ConsoleColor.Yellow;
                case LogCategory.Error:
                    return ConsoleColor.Red;
                case LogCategory.All:
                    return ConsoleColor.Magenta;
                default:
                    return ConsoleColor.White;
            }
        }
    }
}
