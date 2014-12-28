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
                Console.WriteLine(text, parameters);
            }
        }
    }
}