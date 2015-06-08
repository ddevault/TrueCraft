using System;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using TrueCraft.Core;
using TrueCraft.Core.Util;

namespace TrueCraft.Client
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var user = new TrueCraftUser { Username = args[1] };
            var client = new MultiplayerClient(user);
            var game = new TrueCraftGame(client, EndPointParser.ParseEndPoint(args[0]));
            game.Run();
            client.Disconnect();
        }
    }
}
