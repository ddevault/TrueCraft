using System;
using Newtonsoft.Json;
using System.IO;

namespace TrueCraft.Core
{
    public class UserSettings
    {
        public static UserSettings Local { get; set; }

        public static string SettingsPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    ".truecraft", "settings.json");
            }
        }

        public bool AutoLogin { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string LastIP { get; set; }
        public string SelectedTexturePack { get; set; }
        public FavoriteServer[] FavoriteServers { get; set; }
        public WindowResolution WindowResolution { get; set; }

        public UserSettings()
        {
            AutoLogin = false;
            Username = "";
            Password = "";
            LastIP = "";
            SelectedTexturePack = TexturePack.Default.Name;
            FavoriteServers = new FavoriteServer[0];
            WindowResolution = new WindowResolution()
            {
                Width = 1280,
                Height = 720
            };
        }

        public void Load()
        {
            if (File.Exists(SettingsPath))
                JsonConvert.PopulateObject(File.ReadAllText(SettingsPath), this);
        }

        public void Save()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath));
            File.WriteAllText(SettingsPath, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }

    public class FavoriteServer
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class WindowResolution
    {
        public static readonly WindowResolution[] Defaults =
            new WindowResolution[]
            {
                                                            // (from Wikipedia)
                WindowResolution.FromString("800 x 600"),   // SVGA
                WindowResolution.FromString("960 x 640"),   // DVGA
                WindowResolution.FromString("1024 x 600"),  // WSVGA
                WindowResolution.FromString("1024 x 768"),  // XGA
                WindowResolution.FromString("1280 x 1024"), // SXGA
                WindowResolution.FromString("1600 x 1200"), // UXGA
            };

        public static WindowResolution FromString(string str)
        {
            var tmp = str.Split('x');
            return new WindowResolution()
            {
                Width = int.Parse(tmp[0].Trim()),
                Height = int.Parse(tmp[1].Trim())
            };
        }

        public int Width { get; set; }
        public int Height { get; set; }

        public override string ToString()
        {
            return string.Format("{0} x {1}", Width, Height);
        }
    }
}