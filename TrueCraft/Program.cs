using System;
using System.Net;
using System.Threading;

namespace TrueCraft
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            // TODO: Make this more flexible
            var server = new MultiplayerServer();
            server.Start(new IPEndPoint(IPAddress.Any, 25565));
            while (true)
                Thread.Sleep(1000);
        }
    }
}