using System;
using System.Net;
using System.Threading;
using TrueCraft.Core.World;
using TrueCraft.Core.TerrainGen;
using TrueCraft.Core.Logging;

namespace TrueCraft
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            // TODO: Make this more flexible
            var server = new MultiplayerServer();
            server.AddWorld(new World("default", new FlatlandGenerator()));
            server.AddLogProvider(new ConsoleLogProvider());
            server.Start(new IPEndPoint(IPAddress.Any, 25565));
            while (true)
                Thread.Sleep(1000);
        }
    }
}