using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.Core.Windows;
using TrueCraft.API;
using TrueCraft.API.Networking;
using TrueCraft.Core.Networking.Packets;

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

        public override void Handle(IRemoteClient Client, string Alias, string[] Arguments)
        {
            if (Arguments.Length != 0)
            {
                Help(Client, Alias, Arguments);
                return;
            }
            Client.SendMessage(Client.Entity.Position.ToString());
        }

        public override void Help(IRemoteClient Client, string Alias, string[] Arguments)
        {
            Client.SendMessage("/pos: Shows your position.");
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

        public override void Handle(IRemoteClient Client, string Alias, string[] Arguments)
        {
            if (Arguments.Length != 0)
            {
                Help(Client, Alias, Arguments);
                return;
            }
            Client.EnableLogging = !Client.EnableLogging;
        }

        public override void Help(IRemoteClient Client, string Alias, string[] Arguments)
        {
            Client.SendMessage("/pos: Toggles client logging.");
        }
    }

    public class TimeCommand : Command
    {
        public override string Name
        {
            get { return "time"; }
        }

        public override string Description
        {
            get { return "Shows the current time."; }
        }

        public override string[] Aliases
        {
            get { return new string[0]; }
        }

        public override void Handle(IRemoteClient Client, string Alias, string[] Arguments)
        {
            if (Arguments.Length != 0)
            {
                Help(Client, Alias, Arguments);
                return;
            }
            Client.SendMessage(Client.World.Time.ToString());
        }

        public override void Help(IRemoteClient Client, string Alias, string[] Arguments)
        {
            Client.SendMessage("/time: Shows the current time.");
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

        public override void Handle(IRemoteClient Client, string Alias, string[] Arguments)
        {
            if (Arguments.Length != 0)
            {
                Help(Client, Alias, Arguments);
                return;
            }
            Client.QueuePacket(new WindowItemsPacket(0, Client.Inventory.GetSlots()));
        }

        public override void Help(IRemoteClient Client, string Alias, string[] Arguments)
        {
            Client.SendMessage("/reinv: Resends your inventory.");
        }
    }
}