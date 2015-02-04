using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Threading;
using TrueCraft.Core.World;
using TrueCraft.Core.TerrainGen;
using TrueCraft.Core.Logging;
using TrueCraft.API.Logging;
using TrueCraft.API.Server;
using TrueCraft.API;
using TrueCraft.Core.Windows;
using TrueCraft.Commands;
using TrueCraft.API.World;

namespace TrueCraft
{
    class MainClass
    {
        public static CommandManager CommandManager;
        public static void Main(string[] args)
        {
            // TODO: Introduce command line argument parsing to allow for user options
            // TODO: Make this more flexible
            var server = new MultiplayerServer();
            IWorld world;
            try
            {
                // TODO: Save and load levels, with seeds and everything
                world = World.LoadWorld("world");
                world.ChunkProvider = new StandardGenerator();
            }
            catch
            {
                world = new World("default", new StandardGenerator());
                world.Save("world");
            }
            server.AddWorld(world);
            server.AddLogProvider(new ConsoleLogProvider(LogCategory.Notice | LogCategory.Warning | LogCategory.Error | LogCategory.Debug));
            #if DEBUG
            server.AddLogProvider(new FileLogProvider(new StreamWriter("packets.log", false), LogCategory.Packets));
            #endif
            CommandManager = new CommandManager();
            server.ChatMessageReceived += HandleChatMessageReceived;
            server.Start(new IPEndPoint(IPAddress.Any, 25565));
            while (true)
            {
                Thread.Sleep(1000 * 30); // TODO: Allow users to customize world save interval
                foreach (var w in server.Worlds)
                {
                    server.Log(LogCategory.Debug, "Saved world '{0}'", w.Name);
                    w.Save();
                }
            }
        }

        static void HandleChatMessageReceived(object sender, ChatMessageEventArgs e)
        {
            if (e.Message[0] == '/')
            {
                e.PreventDefault = true;
                var messageArray = e.Message.TrimStart('/') // replace with .Remove(0,1) if we're going to allow forward slashes in command aliases
                                            .Split(new[] {' '},
                                                StringSplitOptions.RemoveEmptyEntries);
                CommandManager.HandleCommand(e.Client, messageArray[0], messageArray);
                return;
            }
        }
    }
}