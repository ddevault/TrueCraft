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

        public UserSettings()
        {
            AutoLogin = false;
            Username = "";
            Password = "";
            LastIP = "";
            SelectedTexturePack = TexturePack.Default.Name;
            FavoriteServers = new FavoriteServer[0];
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
}