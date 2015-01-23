using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.Networking;

namespace TrueCraft.API.Server
{
    public class CommandEventArgs : EventArgs
    {
        public CommandEventArgs(IRemoteClient client, string command, string[] arguments)
        {
            Client = client;
            Command = command;
            Arguments = arguments;
        }

        public IRemoteClient Client { get; set; }
        public string Command { get; set; }
        public string[] Arguments { get; set; }
    }
}
