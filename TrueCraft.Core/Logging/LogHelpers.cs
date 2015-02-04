using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueCraft.Core.Logging
{
    public static class LogHelpers
    {
        public static string GetTimestamp(string format = "u", bool utc = true, string trailingSeparator = "\t")
        {
            return (utc ? DateTime.UtcNow : DateTime.Now).ToString(format) + trailingSeparator;
        }
    }
}
