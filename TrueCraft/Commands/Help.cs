using System;
using TrueCraft.API.Networking;
using TrueCraft.API;
using TrueCraft.API.Server;

namespace TrueCraft
{
    public class Help : ICommandHandler
    {
        public string Command 
        { 
            get
            { 
                return "help"; 
            }
        }

        public string Documentation 
        {
            get
            {
                return "Get information about a specific command.\n" +
                    "Usage: /help <command>";
            }
        }

        public int Arguments
        {
            get
            {
                return 1;
            }
        }


        public void Handle(ChatMessageEventArgs e, string[] Arguments)
        {
            if (Arguments.Length == this.Arguments)
            {
                string Documentation = GetCommandDocumentation (Arguments [0]);
                foreach(string Line in Documentation.Split('\n'))
                {
                    e.Client.SendMessage (Line);
                }
            } 
        }

        private string GetCommandDocumentation(string Command)
        {
            string Documentation = "No documentation found!";
            foreach (ICommandHandler i in MainClass.CommandHandlers)
            {
                if (i.Command.ToLower () == Command.ToLower ())
                {
                    Documentation = i.Documentation;
                    break;
                }
            }
            return Documentation;
        }
    }
}

