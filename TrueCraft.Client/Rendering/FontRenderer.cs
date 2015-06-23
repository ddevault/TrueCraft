using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrueCraft.API;

namespace TrueCraft.Client.Rendering
{
    /// <summary>
    /// Represents a font renderer.
    /// </summary>
    public class FontRenderer
    {
        /// <summary>
        /// 
        /// </summary>
        public Font[] Fonts { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="font"></param>
        public FontRenderer(Font font)
        {
            if (font == null)
                throw new ArgumentNullException("font");

            Fonts = new Font[]
            {
                font
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="regular"></param>
        /// <param name="bold"></param>
        /// <param name="strikethrough"></param>
        /// <param name="underline"></param>
        /// <param name="italic"></param>
        public FontRenderer(Font regular, Font bold, Font strikethrough, Font underline, Font italic)
        {
            if (regular == null)
                throw new ArgumentNullException("regular");

            Fonts = new Font[]
            {
                regular,
                bold ?? regular,
                strikethrough ?? regular,
                underline ?? regular,
                italic ?? regular
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="text"></param>
        /// <param name="scale"></param>
        public void DrawText(SpriteBatch spriteBatch, int x, int y, string text, float scale = 1.0f)
        {
            var dx = x;
            var dy = y;
            var color = Color.White;
            var font = Fonts[0];

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '§')
                {
                    i++;
                    var code = string.Format("§{0}", text[i]);
                    if (ChatFormat.IsValid(code))
                        font = GetFont(code);
                    else
                        color = GetColor(code);
                }
                else
                {
                    var glyph = font.GetGlyph(text[i]);
                    if (glyph != null)
                    {
                        var sourceRectangle = new Rectangle(glyph.X, glyph.Y, glyph.Width, glyph.Height);
                        var destRectangle = new Rectangle(
                            dx + (int)(glyph.XOffset * scale),
                            dy + (int)(glyph.YOffset * scale),
                            (int)(glyph.Width * scale),
                            (int)(glyph.Height * scale));
                        var shadowRectangle = new Rectangle(
                            dx + (int)(glyph.XOffset * scale) + 2,
                            dy + (int)(glyph.YOffset * scale) + 2,
                            (int)(glyph.Width * scale),
                            (int)(glyph.Height * scale));

                        spriteBatch.Draw(font.GetTexture(glyph.Page), shadowRectangle, sourceRectangle, new Color(63, 63, 21));
                        spriteBatch.Draw(font.GetTexture(glyph.Page), destRectangle, sourceRectangle, color);
                        dx += (int)(glyph.XAdvance * scale);
                    }
                }
            }
        }

        private Font GetFont(string formatCode)
        {
            // If we are a mono-font renderer, we don't actually care about formatting codes.
            if (Fonts.Length == 1)
                return Fonts[0];

            // Otherwise, determine which font to switch to.
            formatCode = formatCode.ToLowerInvariant();

            switch (formatCode)
            {
                case ChatFormat.Obfuscated: // We don't support obfuscated text yet.
                    return Fonts[(int)FontStyle.Regular];

                case ChatFormat.Bold:
                    return Fonts[(int)FontStyle.Bold];

                case ChatFormat.Strikethrough:
                    return Fonts[(int)FontStyle.Strikethrough];

                case ChatFormat.Underline:
                    return Fonts[(int)FontStyle.Underline];

                case ChatFormat.Italic:
                    return Fonts[(int)FontStyle.Italic];

                case ChatFormat.Reset:
                    return Fonts[(int)FontStyle.Regular];

                default:
                    return Fonts[0];
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
