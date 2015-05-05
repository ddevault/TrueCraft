using System;
using System.Linq;
using System.Text.RegularExpressions;
using TrueCraft.Core.Windows;
using TrueCraft.API;
using TrueCraft.API.Networking;

namespace TrueCraft.Commands
{
    public class GiveCommand : Command
    {
        public override string Name
        {
            get { return "give"; }
        }

        public override string Description
        {
            get { return "Give the specified player an amount of items."; }
        }

        public override string[] Aliases
        {
            get { return new string[1]{ "i" }; }
        }

        public override void Handle(IRemoteClient client, string alias, string[] arguments)
        {
            if (arguments.Length < 3)
            {
                Help(client, alias, arguments);
                return;
            }

            string  username    = arguments[1],
                    itemid      = arguments[2],
                    amount      = "1";

            if(arguments.Length >= 4)
                    amount = arguments[3];
            
            var receivingPlayer =
                client.Server.Clients.FirstOrDefault(c => String.Equals(c.Username, username, StringComparison.CurrentCultureIgnoreCase));

            short id;
            int count;

            if (short.TryParse(itemid, out id) && Int32.TryParse(amount, out count))
            {
                if (receivingPlayer == null)
                {
                    client.SendMessage("No client with the username \"" + username + "\" was found.");
                    return;
                }

                if (client.Server.ItemRepository.GetItemProvider(id) == null)
                {
                    client.SendMessage("Invalid item id \"" + id + "\".");
                    return;
                }

                var inventory = receivingPlayer.Inventory as InventoryWindow;
                if (inventory != null)
                {
                    sbyte toAdd;
                    while (count > 0)
                    {
                        if (count >= 64)
                            toAdd = 64;
                        else
                            toAdd = (sbyte)count;

                        count -= toAdd;

                        inventory.PickUpStack(new ItemStack(id, toAdd));
                    }
                }
            }
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("Correct usage is /" + alias + " <User> <Item ID> [Amount]");
        }
    }
}
