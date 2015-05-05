using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.Networking;
using TrueCraft.API.Server;

namespace TrueCraft.Commands
{
    public class HelpCommand : Command
    {
        public override string Name
        {
            get { return "help"; }
        }

        public override string Description
        {
            get { return "Command help menu."; }
        }

        public override void Handle(IRemoteClient Client, string alias, string[] arguments)
        {
            if (arguments.Length < 1)
            {
                Help(Client, alias, arguments);
                return;
            }

            string Identifier = arguments[0];
            ICommand Found = null;
            if ((Found = Program.CommandManager.FindByName(Identifier)) != null)
            {
                Found.Handle(Client, Identifier, new string[0]);
                return;
            }
            else if ((Found = Program.CommandManager.FindByAlias(Identifier)) != null)
            {
                Found.Help(Client, Identifier, new string[0]);
                return;
            }

            int PageNumber = 0;
            if (int.TryParse(Identifier, out PageNumber))
            {
                HelpPage(Client, PageNumber);
                return;
            }
            Help(Client, alias, arguments);
        }

        public void HelpPage(IRemoteClient Client, int Page)
        {
            int PerPage = 5;
            int Pages = (int)Math.Floor((double)(Program.CommandManager.Commands.Count / PerPage));
            if ((Program.CommandManager.Commands.Count % PerPage) > 0)
                Pages++;

            if (Page < 1 || Page > Pages)
                Page = 1;

            int StartingIndex = (Page - 1) * PerPage;
            Client.SendMessage("--Help Page " + Page + " of " + Pages + "--");
            for (int i = 0; i < PerPage; i++)
            {
                int Index = StartingIndex + i;
                if (Index > Program.CommandManager.Commands.Count - 1)
                {
                    break;
                }
                ICommand C = Program.CommandManager.Commands[Index];
                Client.SendMessage("/" + C.Name + " - " + C.Description);
            }
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("Correct usage is /" + alias + " <page#/command> [command arguments]");
        }
    }
}