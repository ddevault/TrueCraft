using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;
using TrueCraft.API.Networking;
using System.Threading;
using TrueCraft.Core.Networking;
using System.Linq;
using TrueCraft.Core.Networking.Packets;
using TrueCraft.Client.Events;
using TrueCraft.Core.Logic;
using TrueCraft.API.Entities;
using TrueCraft.API;
using System.ComponentModel;
using TrueCraft.Core;

namespace TrueCraft.Client
{
    public delegate void PacketHandler(IPacket packet, MultiplayerClient client);

    public class MultiplayerClient : IAABBEntity, INotifyPropertyChanged // TODO: Make IMultiplayerClient and so on
    {
        public event EventHandler<ChatMessageEventArgs> ChatMessage;
        public event EventHandler<ChunkEventArgs> ChunkModified;
        public event EventHandler<ChunkEventArgs> ChunkLoaded;
        public event EventHandler<ChunkEventArgs> ChunkUnloaded;
        public event PropertyChangedEventHandler PropertyChanged;

        public TrueCraftUser User { get; set; }
        public ReadOnlyWorld World { get; private set; }
        public PhysicsEngine Physics { get; set; }
        public bool LoggedIn { get; internal set; }

        private TcpClient Client { get; set; }
        private IMinecraftStream Stream { get; set; }
        private PacketReader PacketReader { get; set; }
        private BlockingCollection<IPacket> PacketQueue { get; set; }
        private Thread NetworkWorker { get; set; }
        private readonly PacketHandler[] _packetHandlers;

        public MultiplayerClient(TrueCraftUser user)
        {
            User = user;
            Client = new TcpClient();
            PacketQueue = new BlockingCollection<IPacket>(new ConcurrentQueue<IPacket>());
            PacketReader = new PacketReader();
            PacketReader.RegisterCorePackets();
            NetworkWorker = new Thread(DoNetwork);
            _packetHandlers = new PacketHandler[0x100];
            Handlers.PacketHandlers.RegisterHandlers(this);
            World = new ReadOnlyWorld();
            var repo = new BlockRepository();
            repo.DiscoverBlockProviders();
            World.World.BlockRepository = repo;
            Physics = new PhysicsEngine(World, repo);
        }

        public void RegisterPacketHandler(byte packetId, PacketHandler handler)
        {
            _packetHandlers[packetId] = handler;
        }

        public void Connect(IPEndPoint endPoint)
        {
            Client.BeginConnect(endPoint.Address, endPoint.Port, ConnectionComplete, null);
        }

        public void Disconnect()
        {
            NetworkWorker.Abort();
            new DisconnectPacket("Disconnecting").WritePacket(Stream);
            Stream.BaseStream.Flush();
            Client.Close();
        }

        public void QueuePacket(IPacket packet)
        {
            PacketQueue.Add(packet);
        }

        private void ConnectionComplete(IAsyncResult result)
        {
            Client.EndConnect(result);
            Stream = new MinecraftStream(new BufferedStream(Client.GetStream()));
            NetworkWorker.Start();
            Physics.AddEntity(this);
            QueuePacket(new HandshakePacket(User.Username));
        }

        private void DoNetwork()
        {
            bool idle = true;
            while (true)
            {
                IPacket packet;
                var limit = DateTime.Now.AddMilliseconds(500);
                while (Client.Available != 0 && DateTime.Now < limit)
                {
                    idle = false;
                    packet = PacketReader.ReadPacket(Stream, false);
                    if (_packetHandlers[packet.ID] != null)
                        _packetHandlers[packet.ID](packet, this);
                }
                limit = DateTime.Now.AddMilliseconds(500);
                while (PacketQueue.Any() && DateTime.Now < limit)
                {
                    idle = false;

                    if (!PacketQueue.TryTake(out packet, 100)) continue;

                    PacketReader.WritePacket(Stream, packet);
                    Stream.BaseStream.Flush();
                }
                if (idle)
                    Thread.Sleep(100);
            }
        }

        protected internal void OnChatMessage(ChatMessageEventArgs e)
        {
            if (ChatMessage != null) ChatMessage(this, e);
        }

        protected internal void OnChunkLoaded(ChunkEventArgs e)
        {
            if (ChunkLoaded != null) ChunkLoaded(this, e);
        }

        protected internal void OnChunkUnloaded(ChunkEventArgs e)
        {
            if (ChunkUnloaded != null) ChunkUnloaded(this, e);
        }

        protected internal void OnChunkModified(ChunkEventArgs e)
        {
            if (ChunkModified != null) ChunkModified(this, e);
        }

        #region IAABBEntity implementation

        public const double Width = 0.6;
        public const double Height = 1.62;
        public const double Depth = 0.6;

        public void TerrainCollision(Vector3 collisionPoint, Vector3 collisionDirection)
        {
            // This space intentionally left blank
        }

        public BoundingBox BoundingBox
        {
            get
            {
                return new BoundingBox(Position, Position + Size);
            }
        }

        public Size Size
        {
            get { return new Size(Width, Height, Depth); }
        }

        #endregion

        #region IPhysicsEntity implementation

        public bool BeginUpdate()
        {
            return true;
        }

        public void EndUpdate(Vector3 newPosition)
        {
            Position = newPosition;
        }

        public float Yaw { get; set; }
        public float Pitch { get; set; }

        internal Vector3 _Position;
        public Vector3 Position
        {
            get
            {
                return _Position;
            }
            set
            {
                if (_Position != value)
                {
                    QueuePacket(new PlayerPositionAndLookPacket(value.X, value.Y, value.Y + Height, value.Z, Yaw, Pitch, false));
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("Position"));
                }
                _Position = value;
            }
        }

        public Vector3 Velocity { get; set; }

        public float AccelerationDueToGravity
        {
            get
            {
                return 0.08f;
            }
        }

        public float Drag
        {
            get
            {
                return 0.02f;
            }
        }

        public float TerminalVelocity
        {
            get
            {
                return 3.92f;
            }
        }

        #endregion
    }
}