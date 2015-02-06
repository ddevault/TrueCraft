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

                Console.Write(LogHelpers.GetTimestamp());

                Console.ForegroundColor = LogHelpers.GetCategoryColor(category);
                Console.Write(category.ToString());
                Console.ResetColor();

                Console.WriteLine("    " + String.Format(text, parameters));

            }
        }
    }
}