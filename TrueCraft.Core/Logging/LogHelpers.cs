using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.Logging;

namespace TrueCraft.Core.Logging
{
    public static class LogHelpers
    {
        public static string GetTimestamp(bool utc = true, string timeFormat = "yyyy-MM-dd H:mm:ss", string suffix = "    ")
        {
            return (utc ? DateTime.UtcNow : DateTime.Now).ToString(timeFormat) + suffix;
        }

        public static ConsoleColor GetCategoryColor(LogCategory category)
        {
            switch (category)
            {
                case LogCategory.Packets:
                    return ConsoleColor.White;
                case LogCategory.Debug:
                    return ConsoleColor.Cyan;
                case LogCategory.Warning:
                    return ConsoleColor.Yellow;
                case LogCategory.Error:
                    return ConsoleColor.Red;
                case LogCategory.Notice:
                    return ConsoleColor.Green;
                case LogCategory.All:
                    return ConsoleColor.Magenta;
                default:
                    return ConsoleColor.Gray;
            }
        }
    }
}
