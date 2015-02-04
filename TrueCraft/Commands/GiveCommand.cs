using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            if (arguments.Length != 3)
            {
                Help(client, alias, arguments);
                return;
            }
            // TODO: Send items to the client mentioned in the command, not the client issuing the command
            // TODO: Check to make sure an item with that ID actually exists
            short id;
            sbyte count;
            if (short.TryParse(arguments[1], out id) && sbyte.TryParse(arguments[2], out count))
            {
                var inventory = client.Inventory as InventoryWindow;
                if (inventory != null)
                    inventory.PickUpStack(new ItemStack(id, count));
            }
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("Correct usage is /" + alias + "<Partial player name> <Item ID> <Amount>");
        }
    }
}
