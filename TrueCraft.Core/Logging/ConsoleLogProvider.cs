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

                var buffer = Console.BufferWidth;
                var offset = LogHelpers.GetTimestamp().Length;
                var lineLength = buffer - offset;
                // Longest LogCategory is Warning (length is 7 characters)
                // The log will probably mostly contain messages belonging to the
                // category Notice (6 chars). We want a pad of 4 spaces on average
                // and also want the text to be aligned with the last message
                // 7 + 4 = 11 is the max length of (category.ToString() + pad of 4 spaces)
                var lines = getLines(new string(' ', 11) + text, lineLength, parameters); //Add all 11 chars for right alignment
                lines[0] = lines[0].Remove(0, category.ToString().Length); //And remove the amount for category string
                for (int i = 0; i < lines.Length; ++i)
                {
                    if (i != 0) Console.Write(new string(' ', offset));
                    Console.Write(lines[i]);
                }
            }
        }
        private string[] getLines(string text, int lineLength, params object[] parameters)
        {
            text = String.Format(text, parameters);
            string[] lines = new string[(int)Math.Ceiling((double)text.Length / lineLength)];
            int index = 0;
            for (int i = 0; i < text.Length; i += lineLength)
                if (text.Length - i >= lineLength)
                    lines[index++] = text.Substring(i, lineLength);
                else
                    lines[index++] = text.Substring(i, text.Length - i);

            if (lines[lines.Length - 1].Length < lineLength)
                lines[lines.Length - 1] += "\n";
            return lines;
        }
    }
}