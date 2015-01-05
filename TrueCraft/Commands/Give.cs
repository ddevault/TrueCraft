using System;
using TrueCraft.API.Networking;
using TrueCraft.API;
using TrueCraft.API.Server;
using TrueCraft.Core.Windows;

namespace TrueCraft
{
    public class Give : ICommandHandler
    {
        public string Command 
        { 
            get
            {
                return "give"; 
            }
        }

        public string Documentation 
        {
            get
            {
                return "Gives the specified player an item.\n" +
                       "Usage: /give <player> <item> <amount>\n" +
                       "Example: /give Notch 264 64\n" +
                       "This would give the player Notch 64 Diamonds.";
            }
        }

        public int Arguments
        {
            get
            {
                return 3;
            }
        }

        public void Handle(ChatMessageEventArgs e, string[] Arguments)
        {
            if (Arguments.Length != 3)
                return;
            // TODO: Send items to the client mentioned in the command, not the client issuing the command
            // TODO: Check to make sure an item with that ID actually exists
            short id;
            sbyte count;
            if (short.TryParse(Arguments[1], out id) && sbyte.TryParse(Arguments[2], out count))
            {
                var inventory = e.Client.Inventory as InventoryWindow;
                inventory.PickUpStack(new ItemStack(id, count));
            }
        }
    }
}