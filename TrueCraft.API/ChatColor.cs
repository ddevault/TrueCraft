using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueCraft.API
{
    public static class ChatColor
    {
        public const string Black = "§0";
        public const string DarkBlue = "§1";
        public const string DarkGreen = "§2";
        public const string DarkCyan = "§3";
        public const string DarkRed = "§4";
        public const string Purple = "§5";
        public const string Orange = "§6";
        public const string Gray = "§7";
        public const string DarkGray = "§8";
        public const string Blue = "§9";
        public const string BrightGreen = "§a";
        public const string Cyan = "§b";
        public const string Red = "§c";
        public const string Pink = "§d";
        public const string Yellow = "§e";
        public const string White = "§f";

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
