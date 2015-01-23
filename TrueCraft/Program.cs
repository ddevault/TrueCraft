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

namespace TrueCraft
{
    class MainClass
    {
        public static CommandManager CManager;
        private static MultiplayerServer server;
        public static void Main(string[] args)
        {
            // TODO: Make this more flexible
            server = new MultiplayerServer();
            server.AddWorld(new World("default", new StandardGenerator()));
            server.AddLogProvider(new ConsoleLogProvider());
            #if DEBUG
            server.AddLogProvider(new FileLogProvider(new StreamWriter("packets.log", false), LogCategory.Packets));
            #endif
            CManager = new CommandManager();
            server.ChatMessageReceived += HandleChatMessageReceived;
            server.CommandReceived += HandleCommandReceived;
            server.Start(new IPEndPoint(IPAddress.Any, 25565));
            while (true)
                Thread.Sleep(1000);
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

                var cmdArgs = new CommandEventArgs(e.Client, Command, Arguments);
                server.OnCommandReceived(cmdArgs);
                return;
            }
        }

        static void HandleCommandReceived(object sender, CommandEventArgs e)
        {
            CManager.HandleCommand(e.Client, e.Command, e.Arguments);
        }
    }
}