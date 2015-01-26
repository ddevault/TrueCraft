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