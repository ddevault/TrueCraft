using System;
using System.Linq;
using TrueCraft.API.Networking;
using TrueCraft.Core.Networking.Packets;

namespace TrueCraft.Commands
{
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
}