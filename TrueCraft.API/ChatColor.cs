using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueCraft.API
{
    /// <summary>
    /// Provides constants and functions for working with chat colors.
    /// </summary>
    public static class ChatColor
    {
        /// <summary>
        /// The color code for black.
        /// </summary>
        public const string Black = "§0";

        /// <summary>
        /// The color code for dark blue.
        /// </summary>
        public const string DarkBlue = "§1";

        /// <summary>
        /// The color code for dark green.
        /// </summary>
        public const string DarkGreen = "§2";

        /// <summary>
        /// The color code for dark cyan.
        /// </summary>
        public const string DarkCyan = "§3";

        /// <summary>
        /// The color code for dark red.
        /// </summary>
        public const string DarkRed = "§4";

        /// <summary>
        /// The color code for dark purple.
        /// </summary>
        public const string Purple = "§5";

        /// <summary>
        /// The color code for dark orange.
        /// </summary>
        public const string Orange = "§6";

        /// <summary>
        /// The color code for gray.
        /// </summary>
        public const string Gray = "§7";

        /// <summary>
        /// The color code for dark gray.
        /// </summary>
        public const string DarkGray = "§8";

        /// <summary>
        /// The color code for blue.
        /// </summary>
        public const string Blue = "§9";

        /// <summary>
        /// The color code for bright green.
        /// </summary>
        public const string BrightGreen = "§a";

        /// <summary>
        /// The color code for cyan.
        /// </summary>
        public const string Cyan = "§b";

        /// <summary>
        /// The color code for red.
        /// </summary>
        public const string Red = "§c";

        /// <summary>
        /// The color code for pink.
        /// </summary>
        public const string Pink = "§d";

        /// <summary>
        /// The color code for yellow.
        /// </summary>
        public const string Yellow = "§e";

        /// <summary>
        /// The color code for white.
        /// </summary>
        public const string White = "§f";

        /// <summary>
        /// Removes the color codes from the specified string.
        /// </summary>
        /// <param name="text">The string to remove color codes from.</param>
        /// <returns></returns>
        public static string RemoveColors(string text)
        {
            var sb = new StringBuilder(text.Length);
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '§')
                {
                    i++;
                    continue;
                }
                sb.Append(text[i]);
            }
            return sb.ToString();
        }
    }
}
