using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.Networking;

namespace TrueCraft.Commands
{
    public class PingCommand : Command
    {
        public override string Name
        {
            get { return "ping"; }
        }

        public override string Description
        {
            get { return "Ping pong"; }
        }

        public override void Handle(IRemoteClient Client, string Alias, string[] Arguments)
        {
            Client.SendMessage("Pong!");
        }

        public override void Help(IRemoteClient Client, string Alias, string[] Arguments)
        {
            Client.SendMessage("Correct usage is /" + Alias);
        }
    }
}