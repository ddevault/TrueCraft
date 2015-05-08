using TrueCraft.API.Networking;

namespace TrueCraft.Commands
{
    public class GiveMeCommand : GiveCommand
    {
        public override string Name
        {
            get { return "giveme"; }
        }

        public override string[] Aliases
        {
            get { return new string[0]; }
        }

        public override string Description
        {
            get { return "Give yourself an amount of items."; }
        }

        public override void Handle(IRemoteClient client, string alias, string[] arguments)
        {
            if (arguments.Length < 1)
            {
                Help(client, alias, arguments);
                return;
            }

            string itemid = arguments[0],
                amount = "1";

            if (arguments.Length >= 2)
                amount = arguments[1];

            var receivingPlayer = client;

            if (!GiveItem(receivingPlayer, itemid, amount, client))
            {
                Help(client, alias, arguments);
            }
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("Correct usage is /" + alias + " <Item ID> [Amount]");
        }
    }
}