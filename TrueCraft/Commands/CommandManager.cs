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
            Commands.Add(new LogCommand());
        }

        /// <summary>
        ///     Tries to find the specified command by first performing a
        ///     case-insensitive search on the command names, then a
        ///     case-sensitive search on the aliases.
        /// </summary>
        /// <param name="client">Client which called the command</param>
        /// <param name="alias">Case-insensitive name or case-sensitive alias of the command</param>
        /// <param name="arguments"></param>
        public void HandleCommand(IRemoteClient client, string alias, string[] arguments)
        {
            ICommand foundCommand = FindByName(alias) ?? FindByName(alias);
            if (foundCommand == null)
            {
                client.SendMessage("Unable to locate the command \"" + alias + "\". It might be in a different server!");
                return;
            }
            foundCommand.Handle(client, alias, arguments);
        }

        public ICommand FindByName(string name)
        {
            return Commands.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public ICommand FindByAlias(string alias)
        {
            // uncomment below if alias searching should be case-insensitive
            return Commands.FirstOrDefault(c => c.Aliases.Contains(alias /*, StringComparer.OrdinalIgnoreCase*/));
        }
    }
}