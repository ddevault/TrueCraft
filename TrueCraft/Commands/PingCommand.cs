using TrueCraft.API.Networking;

namespace TrueCraft.Commands
{
    public class PingCommand : Command
    {
        public override string Name
        {
            get { return "ping"; }
        }

        public override string Description
        {
            get { return "Ping pong"; }
        }

        public override void Handle(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("Pong!");
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("Correct usage is /" + alias);
        }
    }
}