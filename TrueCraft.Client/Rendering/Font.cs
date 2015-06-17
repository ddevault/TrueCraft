using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TrueCraft.Client.Rendering
{
    /// <summary>
    /// Represents a font.
    /// </summary>
    public class Font
    {
        private FontFile _definition;
        private Texture2D[] _textures;
        private Dictionary<char, FontChar> _glyphs;

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public FontStyle Style { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentManager"></param>
        /// <param name="name"></param>
        /// <param name="style"></param>
        public Font(ContentManager contentManager, string name, FontStyle style = FontStyle.Regular)
        {
            Name = name;
            Style = style;

            LoadContent(contentManager);
            GenerateGlyphs();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public Texture2D GetTexture(int page = 0)
        {
            return _textures[page];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public FontChar GetGlyph(char ch)
        {
            FontChar glyph = null;
            _glyphs.TryGetValue(ch, out glyph);
            return glyph;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentManager"></param>
        private void LoadContent(ContentManager contentManager)
        {
            var definitionPath = string.Format("{0}_{1}.fnt", Name, Style);
            using (var contents = File.OpenRead(Path.Combine(contentManager.RootDirectory, definitionPath)))
                _definition = FontLoader.Load(contents);

            // We need to support multiple texture pages for more than plain ASCII text.
            _textures = new Texture2D[_definition.Pages.Count];
            for (int i = 0; i < _definition.Pages.Count; i++)
            {
                var texturePath = string.Format("{0}_{1}_{2}.png", Name, Style, i);
                _textures[i] = contentManager.Load<Texture2D>(texturePath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void GenerateGlyphs()
        {
            _glyphs = new Dictionary<char, FontChar>();
            foreach (var glyph in _definition.Chars)
            {
                char c = (char)glyph.ID;
                _glyphs.Add(c, glyph);
            }
        }
    }
}
