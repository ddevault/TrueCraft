using System;
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
                Console.Write(LogHelpers.GetTimestamp());
                ConsoleColor currentColor = Console.ForegroundColor;
                Console.ForegroundColor = LogHelpers.GetCategoryColor(category);
                Console.Write(category.ToString());
                // Better to restore original than ResetColor
                Console.ForegroundColor = currentColor;
                // TODO: Check Console.BufferWidth and indent wrapping text onto the same level as the end of the timestamp
                // Longest LogCategory is Warning (length is 7 characters)
                // The log will probably mostly contain messages belonging to the
                // category Notice (6 chars). We want a pad of 4 spaces on average
                // and also want the text to be aligned with the last message
                // 7 + 4 = 11 is the max length of (category.ToString() + pad of 4 spaces)
                Console.WriteLine(new string(' ', 11 - category.ToString().Length) + text, parameters);
            }
        }
    }
}