using Microsoft.Xna.Framework.Graphics;
using System;
using TrueCraft.Core;
using Ionic.Zip;

namespace TrueCraft.Client.Rendering
{
    public static class TexturePackExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="entryName"></param>
        /// <returns></returns>
        public static Texture2D GetTexture(this TexturePack instance, GraphicsDevice graphicsDevice, string entryName)
        {
            ZipEntry entry = null;
            foreach (var item in instance.Archive.Entries)
            {
                if (item.FileName == entryName)
                {
                    entry = item;
                    break;
                }
            }

            if (entry == null)
                return null;

            Texture2D texture = null;
            using (var reader = entry.OpenReader())
                texture = Texture2D.FromStream(graphicsDevice, reader);
            return texture;
        }
    }
}
