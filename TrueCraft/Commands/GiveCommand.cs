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

        public override void Handle(IRemoteClient Client, string Alias, string[] Arguments)
        {
            if (Arguments.Length != 4)
            {
                Help(Client, Alias, Arguments);
                return;
            }
            
            IRemoteClient receiver = Client.Server.Clients.SingleOrDefault(c => c.Username == Arguments[1]);
            short id;
            sbyte count;
            if (short.TryParse(Arguments[2], out id) && sbyte.TryParse(Arguments[3], out count))
            {
                if (receiver == null) {
                    Client.SendMessage("No client with the username \"" + Arguments[1] + "\" was found.");
                    return;
                }

                if (Client.Server.ItemRepository.GetItemProvider(id) == null) {
                    Client.SendMessage("Invalid item id \"" + id + "\".");
                    return;
                }

                var inventory = receiver.Inventory as InventoryWindow;
                inventory.PickUpStack(new ItemStack(id, count));
            }
        }

        public override void Help(IRemoteClient Client, string Alias, string[] Arguments)
        {
            Client.SendMessage("Correct usage is /" + Alias + " <User> <Item ID> <Amount>");
        }
    }
}
