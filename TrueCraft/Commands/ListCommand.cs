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
            get { return "Returns a list of all online players"; }
        }

        public override void Handle(IRemoteClient client, string alias, string[] arguments)
        {
            string toSend = "Current online players: ";
            foreach (IRemoteClient i in client.Server.Clients)
            {
                toSend = toSend + i.Username + ", ";
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