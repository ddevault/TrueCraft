using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.Server;
using TrueCraft.API.Networking;

namespace TrueCraft.Commands
{
    public abstract class Command : ICommand
    {
        public abstract string Name { get; }

        public abstract string Description { get; }

        public virtual string[] Aliases { get { return new string[0]; } }

        public virtual void Handle(IRemoteClient Client, string alias, string[] arguments) { Help(Client, alias, arguments); }

        public virtual void Help(IRemoteClient client, string alias, string[] arguments) { client.SendMessage("Command \"" + alias + "\" is not functional!"); }
    }
}
