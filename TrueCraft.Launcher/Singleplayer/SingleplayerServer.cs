using System;
using TrueCraft.Core.World;
using System.Threading.Tasks;
using TrueCraft.API.Logging;
using TrueCraft.API;
using System.Net;
using TrueCraft.Core.Logging;
using TrueCraft;

namespace TrueCraft.Launcher.Singleplayer
{
    public class SingleplayerServer
    {
        public delegate void ProgressNotification(double progress, string stage);

        public MultiplayerServer Server { get; set; }
        public World World { get; set; }

        public SingleplayerServer(World world)
        {
            World = world;
            Server = new MultiplayerServer();
            TrueCraft.Program.ServerConfiguration = new ServerConfiguration()
            {
                MOTD = null,
                Singleplayer = true
            };
            world.BlockRepository = Server.BlockRepository;
            Server.AddWorld(world);
            Server.AddLogProvider(new ConsoleLogProvider());
        }

        public void Initialize(ProgressNotification progressNotification = null)
        {
            Server.Log(LogCategory.Notice, "Generating world around spawn point...");
            for (int x = -5; x < 5; x++)
            {
                for (int z = -5; z < 5; z++)
                    World.GetChunk(new Coordinates2D(x, z));
                int progress = (int)(((x + 5) / 10.0) * 100);
                if (progressNotification != null)
                    progressNotification(progress / 100.0, "Generating world...");
                if (progress % 10 == 0)
                    Server.Log(LogCategory.Notice, "{0}% complete", progress + 10);
            }
            Server.Log(LogCategory.Notice, "Simulating the world for a moment...");
            for (int x = -5; x < 5; x++)
            {
                for (int z = -5; z < 5; z++)
                {
                    var chunk = World.GetChunk(new Coordinates2D(x, z));
                    for (byte _x = 0; _x < Chunk.Width; _x++)
                    {
                        for (byte _z = 0; _z < Chunk.Depth; _z++)
                        {
                            for (int _y = 0; _y < chunk.GetHeight(_x, _z); _y++)
                            {
                                var coords = new Coordinates3D(x + _x, _y, z + _z);
                                var data = World.GetBlockData(coords);
                                var provider = World.BlockRepository.GetBlockProvider(data.ID);
                                provider.BlockUpdate(data, data, Server, World);
                            }
                        }
                    }
                }
                int progress = (int)(((x + 5) / 10.0) * 100);
                if (progressNotification != null)
                    progressNotification(progress / 100.0, "Simulating world...");
                if (progress % 10 == 0)
                    Server.Log(LogCategory.Notice, "{0}% complete", progress + 10);
            }
            World.Save();
        }

        public void Start()
        {
            Server.Start(new IPEndPoint(IPAddress.Loopback, 0));
        }

        public void Stop()
        {
            Server.Stop();
        }
    }
}