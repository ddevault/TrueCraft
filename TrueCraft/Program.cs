using System;
using System.Net;
using System.Threading;
using TrueCraft.Core.World;
using TrueCraft.Core.TerrainGen;
using TrueCraft.Core.Logging;
using TrueCraft.API.Logging;
using TrueCraft.API.Server;
using TrueCraft.API;
using TrueCraft.Core.Windows;
using System.IO;
using TrueCraft.Commands;
using TrueCraft.API.World;

namespace TrueCraft
{
    class MainClass
    {
        public static CommandManager CommandManager;
        public static void Main(string[] args)
        {
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
            // TODO: Make this more sophisticated
            if (e.Message.StartsWith("/"))
            {
                e.PreventDefault = true;
                var Message = e.Message.Remove(0, 1);
                var Command = Message.Trim();
                var Arguments = new string[0];
                if (Message.Split(' ').Length > 1)
                {
                    Command = Message.Split(' ')[0];
                    Arguments = Message.Substring(Command.Length).Trim().Split(' ');
                }

                CommandManager.HandleCommand(e.Client, Command, Arguments);
                return;
            }
        }
    }
}