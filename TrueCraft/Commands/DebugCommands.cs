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

        public override void Handle(IRemoteClient Client, string alias, string[] arguments)
        {
            if (arguments.Length != 0)
            {
                Help(Client, alias, arguments);
                return;
            }
            Client.SendMessage(Client.Entity.Position.ToString());
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("/pos: Shows your position.");
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

        public override void Handle(IRemoteClient Client, string alias, string[] arguments)
        {
            if (arguments.Length != 0)
            {
                Help(Client, alias, arguments);
                return;
            }
            Client.EnableLogging = !Client.EnableLogging;
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("/pos: Toggles client logging.");
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

        public override void Handle(IRemoteClient Client, string alias, string[] arguments)
        {
            switch (arguments.Length)
            {
                case 0:
                    Client.SendMessage(Client.World.Time.ToString());
                    break;
                case 2:
                    if (!arguments[0].Equals("set"))
                        Help(Client, alias, arguments);

                    int newTime;

                    if(!Int32.TryParse(arguments[1], out newTime))
                        Help(Client, alias, arguments);

                    Client.World.Time = newTime;

                    Client.SendMessage(string.Format("Setting time to {0}", arguments[1]));

                    foreach (var client in Client.Server.Clients.Where(c => c.World.Equals(Client.World)))
                        client.QueuePacket(new TimeUpdatePacket(newTime));
                    
                    break;
                default:
                    Help(Client, alias, arguments);
                    break;
            }
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("/time: Shows the current time.");
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

        public override void Handle(IRemoteClient Client, string alias, string[] arguments)
        {
            if (arguments.Length != 0)
            {
                Help(Client, alias, arguments);
                return;
            }
            Client.QueuePacket(new WindowItemsPacket(0, Client.Inventory.GetSlots()));
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("/reinv: Resends your inventory.");
        }
    }
}