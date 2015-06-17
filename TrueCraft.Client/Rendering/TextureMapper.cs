using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrueCraft.Core;

namespace TrueCraft.Client.Rendering
{
    /// <summary>
    /// Provides mappings from keys to textures.
    /// </summary>
    public sealed class TextureMapper : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly IDictionary<string, Texture2D> Defaults =
            new Dictionary<string, Texture2D>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphicsDevice"></param>
        public static void LoadDefaults(GraphicsDevice graphicsDevice)
        {
            Defaults.Clear();

            Defaults.Add("items.png", Texture2D.FromStream(graphicsDevice, File.OpenRead("Content/items.png")));
            Defaults.Add("terrain.png", Texture2D.FromStream(graphicsDevice, File.OpenRead("Content/terrain.png")));
        }

        /// <summary>
        /// 
        /// </summary>
        public TexturePack TexturePack { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        private IDictionary<string, Texture2D> Customs { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="texturePack"></param>
        public TextureMapper(GraphicsDevice graphicsDevice, TexturePack texturePack = null)
        {
            if (graphicsDevice == null)
                throw new ArgumentException();

            TexturePack = texturePack;
            Customs = new Dictionary<string, Texture2D>();
            IsDisposed = false;

            if (TexturePack != null)
                LoadTextures(graphicsDevice);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphicsDevice"></param>
        private void LoadTextures(GraphicsDevice graphicsDevice)
        {
            foreach (var entry in TexturePack.Archive.Entries)
            {
                // Make sure to 'silence' errors loading custom texture packs;
                // they're unimportant as we can just use default textures.
                try
                {
                    var key = entry.FileName;
                    using (var stream = entry.OpenReader())
                        Customs.Add(key, Texture2D.FromStream(graphicsDevice, stream));
                }
                catch { }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Texture2D GetTexture(string key)
        {
            Texture2D result = null;
            TryGetTexture(key, out result);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="texture"></param>
        /// <returns></returns>
        public bool TryGetTexture(string key, out Texture2D texture)
        {
            // -> Try to load from external texture pack
            // -> Try to load from default texture pack
            // -> Fail gracefully

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException();

            bool hasTexture = false;
            texture = null;
            if (TexturePack != null)
            {
                Texture2D customTexture = null;
                var inCustom = Customs.TryGetValue(key, out customTexture);
                texture = (inCustom) ? customTexture : null;
                hasTexture = inCustom;
            }

            if (!hasTexture)
            {
                Texture2D defaultTexture = null;
                var inDefault = TextureMapper.Defaults.TryGetValue(key, out defaultTexture);
                texture = (inDefault) ? defaultTexture : null;
                hasTexture = inDefault;
            }

            return hasTexture;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
                return;

            foreach (var pair in Customs)
                pair.Value.Dispose();

            Customs.Clear();
            Customs = null;
            TexturePack = null;
            IsDisposed = true;
        }
    }
}
