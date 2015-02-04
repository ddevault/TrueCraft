using System;
using System.Collections.Generic;
using System.Linq;
using TrueCraft.API.Logging;

namespace TrueCraft.Core.Logging
{
    public class ConsoleLogProvider : ILogProvider
    {
        public LogCategory EnabledCategories { get; set; }

        public ConsoleLogProvider(LogCategory enabledCategories = LogCategory.Notice | LogCategory.Warning | LogCategory.Error)
        {
            EnabledCategories = enabledCategories;
        }

        public void Log(LogCategory category, string text, params object[] parameters)
        {
            if ((EnabledCategories & category) != 0)
            {
                // TODO: Colored output labels after timestamp for each LogCategory
                // TODO: If the text doesn't fit the buffer and wraps, indent it onto the same level as the end of the timestamp
                ConsoleColor color;
                switch (category)
                {
                    case LogCategory.Packets:
                        color = ConsoleColor.Green;
                        break;
                    case LogCategory.Debug:
                        color = ConsoleColor.Blue;
                        break;
                    case LogCategory.Warning:
                        color = ConsoleColor.Yellow;
                        break;
                    case LogCategory.Error:
                        color = ConsoleColor.Red;
                        break;
                    case LogCategory.Notice:
                        color = ConsoleColor.White;
                        break;
                    case LogCategory.All:
                        color = ConsoleColor.Magenta;
                        break;
                    default:
                        color = ConsoleColor.White;
                        break;
                }
                Console.Write(LogHelpers.GetTimestamp());
                Console.ForegroundColor = color;
                Console.Write(category.ToString());
                Console.ResetColor();
                Console.WriteLine("    " + String.Format(text, parameters));

            }
        }
    }
}