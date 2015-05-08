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

        public override void Handle(IRemoteClient client, string alias, string[] arguments)
        {
            switch (arguments.Length)
            {
                case 0:
                    client.SendMessage(client.World.Time.ToString());
                    break;
                case 2:
                    if (!arguments[0].Equals("set"))
                        Help(client, alias, arguments);

                    int newTime;

                    if(!Int32.TryParse(arguments[1], out newTime))
                        Help(client, alias, arguments);

                    client.World.Time = newTime;

                    client.SendMessage(string.Format("Setting time to {0}", arguments[1]));

                    foreach (var remoteClient in client.Server.Clients.Where(c => c.World.Equals(client.World)))
                        remoteClient.QueuePacket(new TimeUpdatePacket(newTime));
                    
                    break;
                default:
                    Help(client, alias, arguments);
                    break;
            }
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("/time: Shows the current time.");
        }
    }
}