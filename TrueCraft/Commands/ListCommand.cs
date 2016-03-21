using System.Text;
using TrueCraft.API.Networking;

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
            get { return "Lists players on the server"; }
        }

        public override void Handle(IRemoteClient client, string alias, string[] arguments)
        {
            string toSend = "Currently connected players: ";
            foreach (IRemoteClient i in client.Server.Clients)
            {
                toSend = toSend + i.Username + ", ";
                if (ASCIIEncoding.Unicode.GetByteCount(toSend.ToCharArray) > 32767)
                {
                    client.SendMessage("Too many online players.");
                    return;
                }

            }
            toSend = toSend.Remove(toSend.Length - 2);
            client.SendMessage(toSend);
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("Correct usage is /" + alias);
        }
    }
}