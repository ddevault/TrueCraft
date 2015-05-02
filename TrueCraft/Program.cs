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
using TrueCraft.Core;
using TrueCraft.API;
using YamlDotNet.Serialization;

namespace TrueCraft
{
    public class Program
    {
        public static Configuration Configuration;

        public static CommandManager CommandManager;

        public static MultiplayerServer Server;

        public static void Main(string[] args)
        {
            if (File.Exists("config.yaml"))
            {
                var deserializer = new Deserializer(ignoreUnmatched: true);
                using (var file = File.OpenText("config.yaml"))
                    Configuration = deserializer.Deserialize<Configuration>(file);
            }
            else
                Configuration = new Configuration();
            var serializer = new Serializer();
            using (var writer = new StreamWriter("config.yaml"))
                serializer.Serialize(writer, Configuration);

            Server = new MultiplayerServer();
            if (Configuration.Debug.DeleteWorldOnStartup)
            {
                if (Directory.Exists("world"))
                    Directory.Delete("world", true);
            }
            if (Configuration.Debug.DeletePlayersOnStartup)
            {
                if (Directory.Exists("players"))
                    Directory.Delete("players", true);
            }
            IWorld world;
            try
            {
                world = World.LoadWorld("world");
                world.ChunkProvider = new StandardGenerator();
            }
            catch
            {
                world = new World("default", new StandardGenerator());
                world.BlockRepository = Server.BlockRepository;
                world.Save("world");
            }
            Server.AddWorld(world);
            Server.AddLogProvider(new ConsoleLogProvider(LogCategory.Notice | LogCategory.Warning | LogCategory.Error | LogCategory.Debug));
            #if DEBUG
            Server.AddLogProvider(new FileLogProvider(new StreamWriter("packets.log", false), LogCategory.Packets));
            #endif
            CommandManager = new CommandManager();
            Server.ChatMessageReceived += HandleChatMessageReceived;
            Server.Start(new IPEndPoint(IPAddress.Parse(Configuration.ServerAddress), Configuration.ServerPort));
            Console.CancelKeyPress += HandleCancelKeyPress;
            while (true)
            {
                Thread.Sleep(1000 * 30); // TODO: Allow users to customize world save interval
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
            if (e.Message.StartsWith("/"))
            {
                e.PreventDefault = true;
                var messageArray = e.Message.TrimStart('/')
                    .Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                CommandManager.HandleCommand(e.Client, messageArray[0], messageArray);
                return;
            }
        }
    }
}