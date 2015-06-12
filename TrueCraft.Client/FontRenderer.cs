using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TrueCraft.API;

namespace TrueCraft.Client
{
    public class FontRenderer
    {
        public FontRenderer(FontFile fontFile, Texture2D fontTexture)
        {
            _fontFile = fontFile;
            _texture = fontTexture;
            _characterMap = new Dictionary<char, FontChar>();

            foreach (var fontCharacter in _fontFile.Chars)
            {
                char c = (char)fontCharacter.ID;
                _characterMap.Add(c, fontCharacter);
            }
        }

        private Dictionary<char, FontChar> _characterMap;
        private FontFile _fontFile;
        private Texture2D _texture;

        public void DrawText(SpriteBatch spriteBatch, int x, int y, string text)
        {
            var dx = x;
            var dy = y;
            var color = Color.White;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '§')
                {
                    i++;
                    var code = string.Format("§{0}", text[i]);
                    color = GetColor(code);
                }
                else
                {
                    FontChar fc;
                    if (_characterMap.TryGetValue(text[i], out fc))
                    {
                        var sourceRectangle = new Rectangle(fc.X, fc.Y, fc.Width, fc.Height);
                        var position = new Vector2(dx + fc.XOffset, dy + fc.YOffset);

                        spriteBatch.Draw(_texture, position, sourceRectangle, color);
                        dx += fc.XAdvance;
                    }
                }
            }
        }

        // RGB values taken from http://minecraft.gamepedia.com/Formatting_codes
        private static Color GetColor(string colorCode)
        {
            colorCode = colorCode.ToLowerInvariant();

            switch (colorCode)
            {
                case ChatColor.Black:
                    return new Color(0, 0, 0);

                case ChatColor.DarkBlue:
                    return new Color(0, 0, 170);

                case ChatColor.DarkGreen:
                    return new Color(0, 170, 0);

                case ChatColor.DarkCyan:
                    return new Color(0, 170, 170);

                case ChatColor.DarkRed:
                    return new Color(170, 0, 0);

                case ChatColor.Purple:
                    return new Color(170, 0, 170);

                case ChatColor.Orange:
                    return new Color(255, 170, 0);

                case ChatColor.Gray:
                    return new Color(170, 170, 170);

                case ChatColor.DarkGray:
                    return new Color(85, 85, 85);

                case ChatColor.Blue:
                    return new Color(85, 85, 255);

                case ChatColor.BrightGreen:
                    return new Color(85, 255, 85);

                case ChatColor.Cyan:
                    return new Color(85, 255, 255);

                case ChatColor.Red:
                    return new Color(255, 85, 85);

                case ChatColor.Pink:
                    return new Color(255, 85, 255);

                case ChatColor.Yellow:
                    return new Color(255, 255, 85);

                case ChatColor.White:
                    return new Color(255, 255, 255);

                default:
                    break;
            }

            // Technically this means we have an invalid color code,
            // should we throw an exception?
            return Color.White;
        }
    }
}