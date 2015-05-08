using System;
using System.ComponentModel;
using TrueCraft.API;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NodeDeserializers;

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
            ServerPort = 25565;
            ServerAddress = "0.0.0.0";
            WorldSaveInterval = 30;
        }

        [YamlMember(Alias="motd")]
        public string MOTD { get; set; }

        [YamlMember(Alias="serverPort")]
        public int ServerPort {get; set; }

        [YamlMember(Alias="serverAddress")]
        public string ServerAddress { get; set; }

        [YamlMember(Alias = "debug")]
        public DebugConfiguration Debug { get; set; }

        [YamlMember(Alias = "worldSaveInterval")]
        public int WorldSaveInterval { get; set; }

    }
}