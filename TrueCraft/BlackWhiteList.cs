using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TrueCraft.Api;
using TrueCraft.API.Logging;
using TrueCraft.API.Server;

namespace TrueCraft
{
    public class PlayerBlackList : PlayerList
    {
        public PlayerBlackList(IMultiplayerServer server)
            : base(server)
        {
            ConfigFileName = @"blacklist.txt";
        }
    }

    public class PlayerWhiteList : PlayerList
    {
        public PlayerWhiteList(IMultiplayerServer server)
            : base(server)
        {
            ConfigFileName = @"whitelist.txt";
        }
    }

    public abstract class PlayerList : IPlayerList
    {
        private IList<string> _players;

        protected PlayerList(IMultiplayerServer server)
        {
            Server = server;
        }

        public string ConfigFileName { get; set; }
        public IMultiplayerServer Server { get; set; }
        public IList<string> Players
        {
            get { return _players ?? (_players = LoadPlayersFromFile(ConfigFileName)); }
        }

        private IList<string> LoadPlayersFromFile(string configFileName)
        {
            try
            {
                if (!File.Exists(configFileName))
                    File.Create(configFileName).Close();

                using (var sr = new StreamReader(configFileName))
                {
                    return sr.ReadToEnd().Replace("\r", "").Split('\n').ToList();
                }
            }
            catch (Exception e)
            {
                Server.Log(LogCategory.Error, "The player list file could not be read: " + e.Message);
                return null;
            }
        }

        public override string ToString()
        {
            return string.Format("Players: {0}", _players);
        }
    }
}
