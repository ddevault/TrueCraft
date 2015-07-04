using TrueCraft.API;
using YamlDotNet.Serialization;

namespace TrueCraft
{
    public class ServerConfiguration : Configuration
    {
        public class DebugConfiguration
        {
            public DebugConfiguration()
            {
                DeleteWorldOnStartup = false;
                DeletePlayersOnStartup = false;
            }

            [YamlMember(Alias = "deleteWorldOnStartup")]
            public bool DeleteWorldOnStartup { get; set; }

            [YamlMember(Alias = "deletePlayersOnStartup")]
            public bool DeletePlayersOnStartup { get; set; }
        }

        public ServerConfiguration()
        {
            MOTD = "Welcome to TrueCraft!";
            Debug = new DebugConfiguration();
            ServerPort = 25565;
            ServerAddress = "0.0.0.0";
            WorldSaveInterval = 30;
            Singleplayer = false;
            Query = true;
            QueryPort = 25566;
            EnableLighting = true;
        }

        [YamlMember(Alias = "motd")]
        public string MOTD { get; set; }

        [YamlMember(Alias = "bind-port")]
        public int ServerPort {get; set; }

        [YamlMember(Alias = "bind-endpoint")]
        public string ServerAddress { get; set; }

        [YamlMember(Alias = "debug")]
        public DebugConfiguration Debug { get; set; }

        [YamlMember(Alias = "save-interval")]
        public int WorldSaveInterval { get; set; }

        [YamlIgnore]
        public bool Singleplayer { get; set; }

        [YamlMember(Alias = "query-enabled")]
        public bool Query { get; set; }

        [YamlMember(Alias = "query-port")]
        public int QueryPort { get; set; }

        [YamlMember(Alias = "enable-lighting")]
        public bool EnableLighting { get; set; }
    }
}