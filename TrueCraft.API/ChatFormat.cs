using System;
using System.Text;

namespace TrueCraft.API
{
    /// <summary>
    /// Provides constants and functions for working with chat formatting.
    /// </summary>
    public static class ChatFormat
    {
        /// <summary>
        /// The following text should be obfuscated.
        /// </summary>
        public const string Obfuscated = "§k";

        /// <summary>
        /// The following text should be bold.
        /// </summary>
        public const string Bold = "§l";

        /// <summary>
        /// The following text should be striked-through.
        /// </summary>
        public const string Strikethrough = "§m";

        /// <summary>
        /// The following text should be underlined.
        /// </summary>
        public const string Underline = "§n";

        /// <summary>
        /// The following text should be italicized.
        /// </summary>
        public const string Italic = "§o";

        /// <summary>
        /// The following text should be reset to normal.
        /// </summary>
        public const string Reset = "§r";

        /// <summary>
        /// Returns whether the specified chat code is a valid formatting one.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool IsValid(string code)
        {
            if (string.IsNullOrEmpty(code))
                return false;

            var comparison = StringComparison.InvariantCultureIgnoreCase;
            return
                code.Equals(ChatFormat.Obfuscated, comparison) ||
                code.Equals(ChatFormat.Bold, comparison) ||
                code.Equals(ChatFormat.Strikethrough, comparison) ||
                code.Equals(ChatFormat.Underline, comparison) ||
                code.Equals(ChatFormat.Italic, comparison) ||
                code.Equals(ChatFormat.Reset, comparison);
        }

        /// <summary>
        /// Removes any format codes from a chat string.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Remove(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            var builder = new StringBuilder(text.Length);
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '§')
                {
                    i++;
                    var code = new string('§', text[i]);
                    if (IsValid(code))
                        continue;
                    else
                        builder.Append(code);
                }
                builder.Append(text[i]);
            }
            return builder.ToString();
        }
    }
}
