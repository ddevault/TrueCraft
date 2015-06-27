using System.Net;
using System.Threading;
using TrueCraft.Core.World;
using TrueCraft.Core.TerrainGen;
using TrueCraft.Core.Logging;
using TrueCraft.API.Logging;
using TrueCraft.API.Server;
using System.IO;
using TrueCraft.Commands;
using TrueCraft.API.World;
using System;
using TrueCraft.API;
using YamlDotNet.Serialization;

namespace TrueCraft
{
    public class Program
    {
        public static ServerConfiguration ServerConfiguration;

        public static CommandManager CommandManager;

        public static MultiplayerServer Server;

        public static void Main(string[] args)
        {
            Server = new MultiplayerServer();

            Server.AddLogProvider(new ConsoleLogProvider(LogCategory.Notice | LogCategory.Warning | LogCategory.Error | LogCategory.Debug));
#if DEBUG
            Server.AddLogProvider(new FileLogProvider(new StreamWriter("packets.log", false), LogCategory.Packets));
#endif

            ServerConfiguration = Configuration.LoadConfiguration<ServerConfiguration>("config.yaml");

            if (ServerConfiguration.Debug.DeleteWorldOnStartup)
            {
                if (Directory.Exists("world"))
                    Directory.Delete("world", true);
            }
            if (ServerConfiguration.Debug.DeletePlayersOnStartup)
            {
                if (Directory.Exists("players"))
                    Directory.Delete("players", true);
            }
            IWorld world;
            try
            {
                world = World.LoadWorld("world");
                Server.AddWorld(world);
            }
            catch
            {
                world = new World("default", new StandardGenerator());
                world.BlockRepository = Server.BlockRepository;
                world.Save("world");
                Server.AddWorld(world);
                Server.Log(LogCategory.Notice, "Generating world around spawn point...");
                for (int x = -5; x < 5; x++)
                {
                    for (int z = -5; z < 5; z++)
                        world.GetChunk(new Coordinates2D(x, z));
                    int progress = (int)(((x + 5) / 10.0) * 100);
                    if (progress % 10 == 0)
                        Server.Log(LogCategory.Notice, "{0}% complete", progress + 10);
                }
                Server.Log(LogCategory.Notice, "Simulating the world for a moment...");
                for (int x = -5; x < 5; x++)
                {
                    for (int z = -5; z < 5; z++)
                    {
                        var chunk = world.GetChunk(new Coordinates2D(x, z));
                        for (byte _x = 0; _x < Chunk.Width; _x++)
                        {
                            for (byte _z = 0; _z < Chunk.Depth; _z++)
                            {
                                for (int _y = 0; _y < chunk.GetHeight(_x, _z); _y++)
                                {
                                    var coords = new Coordinates3D(x + _x, _y, z + _z);
                                    var data = world.GetBlockData(coords);
                                    var provider = world.BlockRepository.GetBlockProvider(data.ID);
                                    provider.BlockUpdate(data, data, Server, world);
                                }
                            }
                        }
                    }
                    int progress = (int)(((x + 5) / 10.0) * 100);
                    if (progress % 10 == 0)
                        Server.Log(LogCategory.Notice, "{0}% complete", progress + 10);
                }
            }
            world.Save();
            CommandManager = new CommandManager();
            Server.ChatMessageReceived += HandleChatMessageReceived;
            Server.Start(new IPEndPoint(IPAddress.Parse(ServerConfiguration.ServerAddress), ServerConfiguration.ServerPort));
            Console.CancelKeyPress += HandleCancelKeyPress;
            while (true)
            {
                Thread.Sleep(1000 * ServerConfiguration.WorldSaveInterval);
                foreach (var w in Server.Worlds)
                {
                    w.Save();
                }
            }
        }

        static void HandleCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Server.Stop();
        }

        static void HandleChatMessageReceived(object sender, ChatMessageEventArgs e)
        {
            var message = e.Message;

            if (!message.StartsWith("/") || message.StartsWith("//"))
                SendChatMessage(e.Client.Username, message);
            else
                e.PreventDefault = ProcessChatCommand(e);
        }

        private static void SendChatMessage(string username, string message)
        {
            if (message.StartsWith("//"))
                message = message.Substring(1);

            Server.SendMessage("<{0}> {1}", username, message);
        }

        /// <summary>
        /// Parse sent message as chat command
        /// </summary>
        /// <param name="e"></param>
        /// <returns>true if the command was successfully executed</returns>
        private static bool ProcessChatCommand(ChatMessageEventArgs e)
        {
            var commandWithoutSlash = e.Message.TrimStart('/');
            var messageArray = commandWithoutSlash
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (messageArray.Length <= 0) return false; // command not found

            var alias = messageArray[0];
            var trimmedMessageArray = new string[messageArray.Length - 1];
            if (trimmedMessageArray.Length != 0)
                Array.Copy(messageArray, 1, trimmedMessageArray, 0, messageArray.Length - 1);

            CommandManager.HandleCommand(e.Client, alias, trimmedMessageArray);

            return true;
        }
    }
}
