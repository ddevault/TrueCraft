using System;
using System.Linq;
using TrueCraft.API.Networking;
using TrueCraft.API;

namespace TrueCraft.Commands
{
    public class TellCommand : Command
    {
        public override string Name
        {
            get { return "tell"; }
        }

        public override string Description
        {
            get { return "Whisper someone a message."; }
        }

        public override string[] Aliases
        {
            get { return new string[0]; }
        }

        public override void Handle(IRemoteClient client, string alias, string[] arguments)
        {
            if (arguments.Length < 2)
            {
                Help(client, alias, arguments);
                return;
            }
            
            string username = arguments[0];
            var messageBuilder = new System.Text.StringBuilder();

            for (int i = 1; i < arguments.Length; i++)
                messageBuilder.Append(arguments[i] + " ");
            var message = messageBuilder.ToString();

            var receivingPlayer = GetPlayerByName(client, username);

            if (receivingPlayer == null)
            {
                client.SendMessage("No client with the username \"" + username + "\" was found.");
                return;
            }
            if (receivingPlayer == client)
            {
                client.SendMessage(ChatColor.Red + "You can't send a private message to yourself!");
                return;
            }

            receivingPlayer.SendMessage(ChatColor.Gray + client.Username + " whispers to you: " + message);
        }
        
        protected static IRemoteClient GetPlayerByName(IRemoteClient client, string username)
        {
            var receivingPlayer =
                client.Server.Clients.FirstOrDefault(
                    c => String.Equals(c.Username, username, StringComparison.CurrentCultureIgnoreCase));
            return receivingPlayer;
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("Correct usage is /" + alias + "<player> <message>");
        }
    }
}