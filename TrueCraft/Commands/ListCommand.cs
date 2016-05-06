using System;
using TrueCraft.API.Networking;
using System.Text;

namespace TrueCraft.Commands
{
    public class ListCommand : Command
    {
        public override string Name
        {
            get { return "list"; }
        }

        public override string Description
        {
            get { return "Lists online players"; }
        }

        public override void Handle(IRemoteClient client, string alias, string[] arguments)
        {
            StringBuilder listMessage = new StringBuilder("Currently connected players: ");
            foreach (IRemoteClient c in client.Server.Clients)
            {
                if (listMessage.Length + c.Username.Length + 2 >= 120)
                {
                    client.SendMessage(listMessage.ToString());
                    listMessage.Clear();
                }
                listMessage.AppendFormat("{0}, ", c.Username);
            }
            listMessage.Remove(listMessage.Length - 2, 2);
            client.SendMessage(listMessage.ToString());
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("Correct usage is /" + alias);
        }
    }
}