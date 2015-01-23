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

        public override void Handle(IRemoteClient Client, string Alias, string[] Arguments)
        {
            if (Arguments.Length < 1)
            {
                Help(Client, Alias, Arguments);
                return;
            }

            string Identifier = Arguments[0];
            ICommand Found = null;
            if ((Found = MainClass.CManager.FindByName(Identifier)) != null)
            {
                Found.Handle(Client, Identifier, new string[0]);
                return;
            }
            else if ((Found = MainClass.CManager.FindByAlias(Identifier)) != null)
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
            Help(Client, Alias, Arguments);
        }

        public void HelpPage(IRemoteClient Client, int Page)
        {
            int PerPage = 5;
            int Pages = (int)Math.Floor((double)(MainClass.CManager.Commands.Count / PerPage));
            if ((MainClass.CManager.Commands.Count % PerPage) > 0)
                Pages++;

            if (Page < 1 || Page > Pages)
                Page = 1;

            int StartingIndex = (Page - 1) * PerPage;
            Client.SendMessage("--Help Page " + Page + " of " + Pages + "--");
            for (int i = 0; i < PerPage; i++)
            {
                int Index = StartingIndex + i;
                if (Index > MainClass.CManager.Commands.Count - 1)
                {
                    break;
                }
                ICommand C = MainClass.CManager.Commands[Index];
                Client.SendMessage("/" + C.Name + " - " + C.Description);
            }
        }

        public override void Help(IRemoteClient Client, string Alias, string[] Arguments)
        {
            Client.SendMessage("Correct usage is /" + Alias + " <page#/command> [command arguments]");
        }
    }
}