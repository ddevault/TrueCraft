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

namespace TrueCraft
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            // TODO: Make this more flexible
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
                switch (command)
                {
                    case "ping":
                        e.Client.SendMessage(ChatColor.Blue + "Pong!");
                        break;
                    case "give":
                        if (parameters.Length != 3)
                            break;
                        // TODO: Send items to the client mentioned in the command, not the client issuing the command
                        // TODO: Check to make sure an item with that ID actually exists
                        short id;
                        sbyte count;
                        if (short.TryParse(parameters[1], out id) && sbyte.TryParse(parameters[2], out count))
                        {
                            var inventory = e.Client.Inventory as InventoryWindow;
                            inventory.PickUpStack(new ItemStack(id, count));
                        }
                        break;
                }
            }
        }
    }
}