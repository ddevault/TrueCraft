using System;
using Newtonsoft.Json;
using System.IO;

namespace TrueCraft.Launcher
{
    public class UserSettings
    {
        public static UserSettings Local { get; set; }

        public string SettingsPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".truecraft", "settings.json");
            }
        }

        public bool AutoLogin { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public UserSettings()
        {
            AutoLogin = false;
            Username = "";
            Password = "";
        }

        public void Load()
        {
            if (File.Exists(SettingsPath))
                JsonConvert.PopulateObject(File.ReadAllText(SettingsPath), this);
        }

        public void Save()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath));
            File.WriteAllText(SettingsPath, JsonConvert.SerializeObject(this));
        }
    }
}