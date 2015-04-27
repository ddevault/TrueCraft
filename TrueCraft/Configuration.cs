using System;
using TrueCraft.API;
using YamlDotNet.Serialization;

namespace TrueCraft
{
    public class Configuration
    {
        public class DebugConfiguration
        {
            public DebugConfiguration()
            {
                DeleteWorldOnStartup = false;
                DeletePlayersOnStartup = false;
            }

            [YamlMember(Alias="deleteWorldOnStartup")]
            public bool DeleteWorldOnStartup { get; set; }

            [YamlMember(Alias="deletePlayersOnStartup")]
            public bool DeletePlayersOnStartup { get; set; }
        }

        public Configuration()
        {
            MOTD = ChatColor.Red + "Welcome to TrueCraft!";
            Debug = new DebugConfiguration();
        }

        [YamlMember(Alias="motd")]
        public string MOTD { get; set; }

        [YamlMember(Alias="debug")]
        public DebugConfiguration Debug { get; set; }
    }
}