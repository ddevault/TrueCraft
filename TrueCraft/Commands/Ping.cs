using System;
using TrueCraft.API.Networking;
using TrueCraft.API;
using TrueCraft.API.Server;

namespace TrueCraft
{
    public class Ping : ICommandHandler
    {
        public string Command 
        { 
            get
            { 
                return "ping"; 
            }
        }

        public string Documentation 
        {
            get
            {
                return "Pings the server.\n" +
                       "Usage: /ping";
            }
        }

        public int Arguments
        {
            get
            {
                return 0;
            }
        }

        public void Handle(ChatMessageEventArgs e, string[] Arguments)
        {
            e.Client.SendMessage (ChatColor.Blue + "PONG!");
        }
    }
}

