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
using System.Collections.Generic;
using System.Reflection;

namespace TrueCraft
{
    class MainClass
    {
        public static List<ICommandHandler> CommandHandlers = new List<ICommandHandler> ();

        public static void Main(string[] args)
        {
            // TODO: Make this more flexible
            LoadCommandHandlers (); //Load all command handlers...
            var server = new MultiplayerServer();
            server.AddWorld(new World("default", new StandardGenerator()));
            server.AddLogProvider(new ConsoleLogProvider());
            #if DEBUG
            server.AddLogProvider(new FileLogProvider(new StreamWriter("packets.log", false), LogCategory.Packets));
            #endif
            server.ChatMessageReceived += HandleChatMessageReceived;
            server.Start(new IPEndPoint(IPAddress.Any, 25565));
            while (true)
                Thread.Sleep(1000);
        }
            
        static void LoadCommandHandlers()
        {
            CommandHandlers.Add (new Ping());
            CommandHandlers.Add (new Give());
            CommandHandlers.Add (new Help());
        }

        static void HandleChatMessageReceived(object sender, ChatMessageEventArgs e)
        {
            // TODO: Make this more sophisticated
            if (e.Message.StartsWith("/"))
            {
                e.PreventDefault = true;
                var space = e.Message.IndexOf(' ');
                if (space == -1)
                    space = e.Message.Length;
                var command = e.Message.Substring(1, space - 1);
                var parameters = e.Message.Substring(command.Length + 1).Trim().Split(' ');

                foreach (ICommandHandler h in CommandHandlers) 
                { 
                    if (h.Command.ToLower() == command.ToLower()) 
                    {
                        if (parameters.Length >= h.Arguments)
                            h.Handle (e, parameters);
                        else
                            e.Client.SendMessage ("Incorrect usage. Use /help " + command + " for more information.");
                        break;
                    }
                }
            }
        }
    }
}
