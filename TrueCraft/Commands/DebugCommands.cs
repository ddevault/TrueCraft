using TrueCraft.API.Networking;
using TrueCraft.Core.Networking.Packets;
using TrueCraft.API;

namespace TrueCraft.Commands
{
    public class PositionCommand : Command
    {
        public override string Name
        {
            get { return "pos"; }
        }

        public override string Description
        {
            get { return "Shows your position."; }
        }

        public override string[] Aliases
        {
            get { return new string[0]; }
        }

        public override void Handle(IRemoteClient client, string alias, string[] arguments)
        {
            if (arguments.Length != 0)
            {
                Help(client, alias, arguments);
                return;
            }
            client.SendMessage(client.Entity.Position.ToString());
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("/pos: Shows your position.");
        }
    }

    public class TrashCommand : Command
    {
        public override string Name
        {
            get { return "trash"; }
        }

        public override string Description
        {
            get { return "Discards selected item."; }
        }

        public override string[] Aliases
        {
            get { return new string[0]; }
        }

        public override void Handle(IRemoteClient client, string alias, string[] arguments)
        {
            if (arguments.Length != 0)
            {
                Help(client, alias, arguments);
                return;
            }
            client.Inventory[client.SelectedSlot] = ItemStack.EmptyStack;
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("/trash: Discards selected item.");
        }
    }

    public class WhatCommand : Command
    {
        public override string Name
        {
            get { return "what"; }
        }

        public override string Description
        {
            get { return "Tells you what you're holding."; }
        }

        public override string[] Aliases
        {
            get { return new string[0]; }
        }

        public override void Handle(IRemoteClient client, string alias, string[] arguments)
        {
            if (arguments.Length != 0)
            {
                Help(client, alias, arguments);
                return;
            }
            client.SendMessage(client.SelectedItem.ToString());
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("/what: Tells you what you're holding.");
        }
    }

    public class LogCommand : Command
    {
        public override string Name
        {
            get { return "log"; }
        }

        public override string Description
        {
            get { return "Toggles client logging."; }
        }

        public override string[] Aliases
        {
            get { return new string[0]; }
        }

        public override void Handle(IRemoteClient client, string alias, string[] arguments)
        {
            if (arguments.Length != 0)
            {
                Help(client, alias, arguments);
                return;
            }
            client.EnableLogging = !client.EnableLogging;
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("/pos: Toggles client logging.");
        }
    }
    
    public class ResendInvCommand : Command
    {
        public override string Name
        {
            get { return "reinv"; }
        }

        public override string Description
        {
            get { return "Resends your inventory to the selected client."; }
        }

        public override string[] Aliases
        {
            get { return new string[0]; }
        }

        public override void Handle(IRemoteClient client, string alias, string[] arguments)
        {
            if (arguments.Length != 0)
            {
                Help(client, alias, arguments);
                return;
            }
            client.QueuePacket(new WindowItemsPacket(0, client.Inventory.GetSlots()));
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("/reinv: Resends your inventory.");
        }
    }
}