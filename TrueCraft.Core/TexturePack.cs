using System;
using System.IO;
using Ionic.Zip;

namespace TrueCraft.Core
{
    /// <summary>
    /// Represents a Minecraft 1.7.3 texture pack (.zip archive).
    /// </summary>
    public class TexturePack
    {
        /// <summary>
        /// 
        /// </summary>
        public const string DefaultID = "#Default";

        /// <summary>
        /// 
        /// </summary>
        public static string TexturePackPath
        {
            get
            {
                return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    ".truecraft/texturepacks/");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ZipFile Archive { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public MemoryStream Image { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsCorrupt { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public TexturePack()
        {
            Path = TexturePack.DefaultID;
            Archive = new ZipFile();
            Name = "Default";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public TexturePack(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                MakeDefault();

            Path = path;
            var fileInfo = new FileInfo(path); // A bit weird, but it works.
            Name = fileInfo.Name;
            try { Archive = new ZipFile(path); }
            catch { IsCorrupt = true; }

            GetPackInfo();
        }

        /// <summary>
        /// 
        /// </summary>
        private void MakeDefault()
        {
            Path = TexturePack.DefaultID;
            Archive = new ZipFile();
            Name = "Default";
            Image = null;
            Description = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void GetPackInfo()
        {
            try
            {
                foreach (var entry in Archive.Entries)
                {
                    if (entry.FileName == "pack.txt")
                    {
                        using (var stream = entry.OpenReader())
                        {
                            using (var reader = new StreamReader(stream))
                                Description = reader.ReadToEnd();
                        }
                    }
                    else if (entry.FileName == "pack.png")
                    {
                        using (var stream = entry.OpenReader())
                        {
                            // Better way to do this?
                            var buffer = new byte[entry.UncompressedSize];
                            stream.Read(buffer, 0, buffer.Length);
                            Image = new MemoryStream((int)entry.UncompressedSize);
                            Image.Write(buffer, 0, buffer.Length);

                            // Fixes 'GLib.GException: Unrecognized image file format' on Linux.
                            Image.Seek(0, SeekOrigin.Begin);
                        }
                    }
                }
            }
            catch { IsCorrupt = true; }
        }
    }
}
