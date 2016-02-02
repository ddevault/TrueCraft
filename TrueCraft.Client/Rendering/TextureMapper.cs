using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrueCraft.Core;
using Ionic.Zip;

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

            Defaults.Add("terrain.png", Texture2D.FromStream(graphicsDevice, File.OpenRead("Content/terrain.png")));
            Defaults.Add("gui/items.png", Texture2D.FromStream(graphicsDevice, File.OpenRead("Content/items.png")));
            Defaults.Add("gui/gui.png", Texture2D.FromStream(graphicsDevice, File.OpenRead("Content/gui.png")));
            Defaults.Add("gui/icons.png", Texture2D.FromStream(graphicsDevice, File.OpenRead("Content/icons.png")));
            Defaults.Add("gui/crafting.png", Texture2D.FromStream(graphicsDevice, File.OpenRead("Content/crafting.png")));
            Defaults.Add("gui/furnace.png", Texture2D.FromStream(graphicsDevice, File.OpenRead("Content/furnace.png")));
            Defaults.Add("gui/inventory.png", Texture2D.FromStream(graphicsDevice, File.OpenRead("Content/inventory.png")));
        }

        /// <summary>
        /// 
        /// </summary>
        private GraphicsDevice Device { get; set; }

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
        public TextureMapper(GraphicsDevice graphicsDevice)
        {
            if (graphicsDevice == null)
                throw new ArgumentException();

            Device = graphicsDevice;
            Customs = new Dictionary<string, Texture2D>();
            IsDisposed = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="texture"></param>
        public void AddTexture(string key, Texture2D texture)
        {
            if (string.IsNullOrEmpty(key) || (texture == null))
                throw new ArgumentException();

            if (Customs.ContainsKey(key))
                Customs[key] = texture;
            else
                Customs.Add(key, texture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="texturePack"></param>
        public void AddTexturePack(TexturePack texturePack)
        {
            if (texturePack == null)
                return;

            // Make sure to 'silence' errors loading custom texture packs;
            // they're unimportant as we can just use default textures.
            try
            {
                var archive = new ZipFile(Path.Combine(Paths.TexturePacks, texturePack.Name));
                foreach (var entry in archive.Entries)
                {
                    var key = entry.FileName;
                    if (Path.GetExtension(key) == ".png")
                    {
                        using (var stream = entry.OpenReader())
                        {
                            try
                            {
                                using (var ms = new MemoryStream())
                                {
                                    CopyStream(stream, ms);
                                    ms.Seek(0, SeekOrigin.Begin);
                                    AddTexture(key, Texture2D.FromStream(Device, ms));
                                }
                            }
                            catch (Exception ex) { Console.WriteLine("Exception occured while loading {0} from texture pack:\n\n{1}", key, ex); }
                        }
                    }
                }
            }
            catch { return; }
        }

        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[16*1024];
            int read;
            while((read = input.Read (buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
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
            if (result == null)
                throw new InvalidOperationException();

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
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException();

            bool hasTexture = false;
            texture = null;

            // -> Try to load from custom textures
            Texture2D customTexture = null;
            var inCustom = Customs.TryGetValue(key, out customTexture);
            texture = (inCustom) ? customTexture : null;
            hasTexture = inCustom;

            // -> Try to load from default textures
            if (!hasTexture)
            {
                Texture2D defaultTexture = null;
                var inDefault = TextureMapper.Defaults.TryGetValue(key, out defaultTexture);
                texture = (inDefault) ? defaultTexture : null;
                hasTexture = inDefault;
            }

            // -> Fail gracefully
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

            Customs = null;
            Device = null;
            IsDisposed = true;
        }
    }
}
