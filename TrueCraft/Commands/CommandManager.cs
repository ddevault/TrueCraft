using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.Server;
using TrueCraft.API.Networking;

namespace TrueCraft.Commands
{
    public class CommandManager : ICommandManager
    {
        public IList<ICommand> Commands { get; set; }

        public CommandManager()
        {
            Commands = new List<ICommand>();
            LoadCommands();
        }

        private void LoadCommands()
        {
            Commands.Add(new PingCommand());
            Commands.Add(new GiveCommand());
            Commands.Add(new HelpCommand());
            Commands.Add(new ResendInvCommand());
            Commands.Add(new PositionCommand());
            Commands.Add(new TimeCommand());
        }

        /// <summary>
        /// Handle the specified command if it exists. We run the check twice to separate
        /// actual command names from command aliases to prevent aliases from being prioritized
        /// over other command names.
        /// </summary>
        /// <param name="Client"></param>
        /// <param name="Command"></param>
        /// <param name="Arguments"></param>
        public void HandleCommand(IRemoteClient Client, string Alias, string[] Arguments)
        {
            ICommand Found = null;
            if ((Found = FindByName(Alias)) != null)
            {
                Found.Handle(Client, Alias, Arguments);
                return;
            }
            else if ((Found = FindByAlias(Alias)) != null)
            {
                Found.Handle(Client, Alias, Arguments);
                return;
            }
            Client.SendMessage("Unable to locate the command \"" + Alias + "\". It might be in a different server!");
        }

        public ICommand FindByName(string Name)
        {
            foreach (ICommand C in Commands)
            {
                if (C.Name.ToLower() == Name.ToLower())
                {
                    return C;
                }
            }
            return null;
        }

        public ICommand FindByAlias(string Alias)
        {
            foreach (ICommand C in Commands)
            {
                if (C.Aliases.Contains(Alias))
                {
                    return C;
                }
            }
            return null;
        }
    }
}