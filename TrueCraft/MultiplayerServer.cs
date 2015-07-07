using System;
using TrueCraft.API.Server;
using TrueCraft.API.Networking;
using TrueCraft.Core.Networking;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using TrueCraft.API.World;
using TrueCraft.API.Logging;
using TrueCraft.Core.Networking.Packets;
using TrueCraft.API;
using TrueCraft.Core.Logging;
using TrueCraft.API.Logic;
using TrueCraft.Exceptions;
using TrueCraft.Core.Logic;
using TrueCraft.Core.Lighting;
using TrueCraft.Core.World;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace TrueCraft
{
    public class MultiplayerServer : IMultiplayerServer, IDisposable
    {
        public event EventHandler<ChatMessageEventArgs> ChatMessageReceived;
        public event EventHandler<PlayerJoinedQuitEventArgs> PlayerJoined;
        public event EventHandler<PlayerJoinedQuitEventArgs> PlayerQuit;

        public IAccessConfiguration AccessConfiguration { get; internal set; }

        public IPacketReader PacketReader { get; private set; }
        public IList<IRemoteClient> Clients { get; private set; }
        public IList<IWorld> Worlds { get; private set; }
        public IList<IEntityManager> EntityManagers { get; private set; }
        public IList<WorldLighting> WorldLighters { get; set; }
        public IEventScheduler Scheduler { get; private set; }
        public IBlockRepository BlockRepository { get; private set; }
        public IItemRepository ItemRepository { get; private set; }
        public ICraftingRepository CraftingRepository { get; private set; }
        public bool EnableClientLogging { get; set; }
        public IPEndPoint EndPoint { get; private set; }

        private bool _BlockUpdatesEnabled = true;

        private struct BlockUpdate
        {
            public Coordinates3D Coordinates;
            public IWorld World;
        }
        private Queue<BlockUpdate> PendingBlockUpdates { get; set; }
        public bool BlockUpdatesEnabled
        {
            get
            {
                return _BlockUpdatesEnabled;
            }
            set
            {
                _BlockUpdatesEnabled = value;
                if (_BlockUpdatesEnabled)
                {
                    ProcessBlockUpdates();
                }
            }
        }

        private Timer EnvironmentWorker;
        private TcpListener Listener;
        private readonly PacketHandler[] PacketHandlers;
        private IList<ILogProvider> LogProviders;
        private ConcurrentBag<Tuple<IWorld, IChunk>> ChunksToSchedule;
        internal object ClientLock = new object();
        
        private QueryProtocol QueryProtocol;

        internal bool ShuttingDown { get; private set; }
        
        public MultiplayerServer()
        {
            var reader = new PacketReader();
            PacketReader = reader;
            Clients = new List<IRemoteClient>();
            EnvironmentWorker = new Timer(DoEnvironment);
            PacketHandlers = new PacketHandler[0x100];
            Worlds = new List<IWorld>();
            EntityManagers = new List<IEntityManager>();
            LogProviders = new List<ILogProvider>();
            Scheduler = new EventScheduler(this);
            var blockRepository = new BlockRepository();
            blockRepository.DiscoverBlockProviders();
            BlockRepository = blockRepository;
            var itemRepository = new ItemRepository();
            itemRepository.DiscoverItemProviders();
            ItemRepository = itemRepository;
            BlockProvider.ItemRepository = ItemRepository;
            BlockProvider.BlockRepository = BlockRepository;
            var craftingRepository = new CraftingRepository();
            craftingRepository.DiscoverRecipes();
            CraftingRepository = craftingRepository;
            PendingBlockUpdates = new Queue<BlockUpdate>();
            EnableClientLogging = false;
            QueryProtocol = new TrueCraft.QueryProtocol(this);
            WorldLighters = new List<WorldLighting>();
            ChunksToSchedule = new ConcurrentBag<Tuple<IWorld, IChunk>>();

            AccessConfiguration = Configuration.LoadConfiguration<AccessConfiguration>("access.yaml");

            reader.RegisterCorePackets();
            Handlers.PacketHandlers.RegisterHandlers(this);
        }

        public void RegisterPacketHandler(byte packetId, PacketHandler handler)
        {
            PacketHandlers[packetId] = handler;
        }

        public void Start(IPEndPoint endPoint)
        {
            ShuttingDown = false;
            Listener = new TcpListener(endPoint);
            Listener.Start();
            EndPoint = (IPEndPoint)Listener.LocalEndpoint;

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += AcceptClient;

            if (!Listener.Server.AcceptAsync(args))
                AcceptClient(this, args);
            
            Log(LogCategory.Notice, "Running TrueCraft server on {0}", EndPoint);
            EnvironmentWorker.Change(1000 / 20, 0);
            if(Program.ServerConfiguration.Query)
                QueryProtocol.Start();
        }

        public void Stop()
        {
            ShuttingDown = true;
            Listener.Stop();
            if(Program.ServerConfiguration.Query)
                QueryProtocol.Stop();
            foreach (var w in Worlds)
                w.Save();
            foreach (var c in Clients)
                DisconnectClient(c);
        }

        public void AddWorld(IWorld world)
        {
            Worlds.Add(world);
            world.BlockRepository = BlockRepository;
            world.ChunkGenerated += HandleChunkGenerated;
            world.ChunkLoaded += HandleChunkLoaded;
            world.BlockChanged += HandleBlockChanged;
            var manager = new EntityManager(this, world);
            EntityManagers.Add(manager);
            var lighter = new WorldLighting(world, BlockRepository);
            WorldLighters.Add(lighter);
            foreach (var chunk in world)
                HandleChunkLoaded(world, new ChunkLoadedEventArgs(chunk));
        }

        void HandleChunkLoaded(object sender, ChunkLoadedEventArgs e)
        {
            ChunksToSchedule.Add(new Tuple<IWorld, IChunk>(sender as IWorld, e.Chunk));
        }

        void HandleBlockChanged(object sender, BlockChangeEventArgs e)
        {
            // TODO: Propegate lighting changes to client (not possible with beta 1.7.3 protocol)
            if (e.NewBlock.ID != e.OldBlock.ID || e.NewBlock.Metadata != e.OldBlock.Metadata)
            {
                for (int i = 0, ClientsCount = Clients.Count; i < ClientsCount; i++)
                {
                    var client = (RemoteClient)Clients[i];
                    // TODO: Confirm that the client knows of this block
                    if (client.LoggedIn && client.World == sender)
                    {
                        client.QueuePacket(new BlockChangePacket(e.Position.X, (sbyte)e.Position.Y, e.Position.Z,
                                (sbyte)e.NewBlock.ID, (sbyte)e.NewBlock.Metadata));
                    }
                }
                PendingBlockUpdates.Enqueue(new BlockUpdate { Coordinates = e.Position, World = sender as IWorld });
                ProcessBlockUpdates();
                if (Program.ServerConfiguration.EnableLighting)
                {
                    var lighter = WorldLighters.SingleOrDefault(l => l.World == sender);
                    if (lighter != null)
                    {
                        lighter.EnqueueOperation(new BoundingBox(e.Position, e.Position + Vector3.One), true);
                        lighter.EnqueueOperation(new BoundingBox(e.Position, e.Position + Vector3.One), false);
                    }
                }
            }
        }

        void HandleChunkGenerated(object sender, ChunkLoadedEventArgs e)
        {
            if (Program.ServerConfiguration.EnableLighting)
            {
                var lighter = new WorldLighting(sender as IWorld, BlockRepository);
                lighter.InitialLighting(e.Chunk);
            }
            else
            {
                for (int i = 0; i < e.Chunk.SkyLight.Data.Length; i++)
                {
                    e.Chunk.SkyLight.Data[i] = 0xFF;
                }
            }
            HandleChunkLoaded(sender, e);
        }

        void ScheduleUpdatesForChunk(IWorld world, IChunk chunk)
        {
            int _x = chunk.Coordinates.X * Chunk.Width;
            int _z = chunk.Coordinates.Z * Chunk.Depth;
            for (byte x = 0; x < Chunk.Width; x++)
            {
                for (byte z = 0; z < Chunk.Depth; z++)
                {
                    for (int y = 0; y < chunk.GetHeight(x, z); y++)
                    {
                        var coords = new Coordinates3D(_x + x, y, _z + z);
                        var id = world.GetBlockID(coords);
                        if (id == 0)
                            continue;
                        var provider = BlockRepository.GetBlockProvider(id);
                        provider.BlockLoadedFromChunk(coords, this, world);
                    }
                }
            }
        }

        private void ProcessBlockUpdates()
        {
            if (!BlockUpdatesEnabled)
                return;
            var adjacent = new[]
            {
                Coordinates3D.Up, Coordinates3D.Down,
                Coordinates3D.Left, Coordinates3D.Right,
                Coordinates3D.Forwards, Coordinates3D.Backwards
            };
            while (PendingBlockUpdates.Count != 0)
            {
                var update = PendingBlockUpdates.Dequeue();
                var source = update.World.GetBlockData(update.Coordinates);
                foreach (var offset in adjacent)
                {
                    var descriptor = update.World.GetBlockData(update.Coordinates + offset);
                    var provider = BlockRepository.GetBlockProvider(descriptor.ID);
                    if (provider != null)
                        provider.BlockUpdate(descriptor, source, this, update.World);
                }
            }
        }

        public void AddLogProvider(ILogProvider provider)
        {
            LogProviders.Add(provider);
        }

        public void Log(LogCategory category, string text, params object[] parameters)
        {
            for (int i = 0, LogProvidersCount = LogProviders.Count; i < LogProvidersCount; i++)
            {
                var provider = LogProviders[i];
                provider.Log(category, text, parameters);
            }
        }

        public IEntityManager GetEntityManagerForWorld(IWorld world)
        {
            for (int i = 0; i < EntityManagers.Count; i++)
            {
                var manager = EntityManagers[i] as EntityManager;
                if (manager.World == world)
                    return manager;
            }
            return null;
        }

        public void SendMessage(string message, params object[] parameters)
        {
            var compiled = string.Format(message, parameters);
            var parts = compiled.Split('\n');
            foreach (var client in Clients)
            {
                foreach (var part in parts)
                    client.SendMessage(part);
            }
            Log(LogCategory.Notice, ChatColor.RemoveColors(compiled));
        }

        protected internal void OnChatMessageReceived(ChatMessageEventArgs e)
        {
            if (ChatMessageReceived != null)
                ChatMessageReceived(this, e);
        }

        protected internal void OnPlayerJoined(PlayerJoinedQuitEventArgs e)
        {
            if (PlayerJoined != null)
                PlayerJoined(this, e);
        }

        protected internal void OnPlayerQuit(PlayerJoinedQuitEventArgs e)
        {
            if (PlayerQuit != null)
                PlayerQuit(this, e);
        }

        public void DisconnectClient(IRemoteClient _client)
        {
            var client = (RemoteClient)_client;

            lock (ClientLock)
            {
                Clients.Remove(client);
            }

            if (client.Disconnected)
                return;

            client.Disconnected = true;

            if (client.LoggedIn)
            {
                SendMessage(ChatColor.Yellow + "{0} has left the server.", client.Username);
                GetEntityManagerForWorld(client.World).DespawnEntity(client.Entity);
                GetEntityManagerForWorld(client.World).FlushDespawns();
            }
            client.Save();
            client.Disconnect();
            OnPlayerQuit(new PlayerJoinedQuitEventArgs(client));

            client.Dispose();
        }

        private void AcceptClient(object sender, SocketAsyncEventArgs args)
        {
            try
            {
                var client = new RemoteClient(this, PacketReader, PacketHandlers, args.AcceptSocket);

                lock (ClientLock)
                    Clients.Add(client);
            }
            catch
            {
                // Who cares
            }
            finally
            {
                args.AcceptSocket = null;

                if (!ShuttingDown && !Listener.Server.AcceptAsync(args))
                    AcceptClient(this, args);
            }
        }

        private void DoEnvironment(object discarded)
        {
            if (ShuttingDown)
                return;
            Scheduler.Update();
            foreach (var manager in EntityManagers)
            {
                manager.Update();
            }
            foreach (var lighter in WorldLighters)
            {
                int attempts = 500;
                while (attempts-- > 0 && lighter.TryLightNext())
                {
                }
            }
            Tuple<IWorld, IChunk> t;
            if (ChunksToSchedule.TryTake(out t))
                ScheduleUpdatesForChunk(t.Item1, t.Item2);
            EnvironmentWorker.Change(1000 / 20, 0);
        }

        public bool PlayerIsWhitelisted(string client)
        {
            return AccessConfiguration.Whitelist.Contains(client, StringComparer.CurrentCultureIgnoreCase);
        }

        public bool PlayerIsBlacklisted(string client)
        {
            return AccessConfiguration.Blacklist.Contains(client, StringComparer.CurrentCultureIgnoreCase);
        }

        public bool PlayerIsOp(string client)
        {
            return AccessConfiguration.Oplist.Contains(client, StringComparer.CurrentCultureIgnoreCase);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop();
            }
        }

        ~MultiplayerServer()
        {
            Dispose(false);
        }
    }
}
