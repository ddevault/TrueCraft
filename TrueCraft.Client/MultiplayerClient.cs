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
using System.IO;
using TrueCraft.Core;
using TrueCraft.API.Physics;

namespace TrueCraft.Client
{
    public delegate void PacketHandler(IPacket packet, MultiplayerClient client);

    public class MultiplayerClient : IAABBEntity, INotifyPropertyChanged, IDisposable // TODO: Make IMultiplayerClient and so on
    {
        public event EventHandler<ChatMessageEventArgs> ChatMessage;
        public event EventHandler<ChunkEventArgs> ChunkModified;
        public event EventHandler<ChunkEventArgs> ChunkLoaded;
        public event EventHandler<ChunkEventArgs> ChunkUnloaded;
        public event PropertyChangedEventHandler PropertyChanged;

        private long connected;

        public TrueCraftUser User { get; set; }
        public ReadOnlyWorld World { get; private set; }
        public PhysicsEngine Physics { get; set; }
        public bool LoggedIn { get; internal set; }

        public bool Connected
        {
            get
            {
                return Interlocked.Read(ref connected) == 1;
            }
        }

        private TcpClient Client { get; set; }
        private IMinecraftStream Stream { get; set; }
        private PacketReader PacketReader { get; set; }

        private readonly PacketHandler[] PacketHandlers;

        private SemaphoreSlim sem = new SemaphoreSlim(1, 1);

        private readonly CancellationTokenSource cancel;

        private SocketAsyncEventArgsPool SocketPool { get; set; }

        public MultiplayerClient(TrueCraftUser user)
        {
            User = user;
            Client = new TcpClient();
            PacketReader = new PacketReader();
            PacketReader.RegisterCorePackets();
            PacketHandlers = new PacketHandler[0x100];
            Handlers.PacketHandlers.RegisterHandlers(this);
            World = new ReadOnlyWorld();
            var repo = new BlockRepository();
            repo.DiscoverBlockProviders();
            World.World.BlockRepository = repo;
            World.World.ChunkProvider = new EmptyGenerator();
            Physics = new PhysicsEngine(World, repo);
            SocketPool = new SocketAsyncEventArgsPool(100, 200, 65536);
            connected = 0;
            cancel = new CancellationTokenSource();
        }

        public void RegisterPacketHandler(byte packetId, PacketHandler handler)
        {
            PacketHandlers[packetId] = handler;
        }

        public void Connect(IPEndPoint endPoint)
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += Connection_Completed;
            args.RemoteEndPoint = endPoint;

            if (!Client.Client.ConnectAsync(args))
                Connection_Completed(this, args);
        }

        private void Connection_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                Interlocked.CompareExchange(ref connected, 1, 0);

                Physics.AddEntity(this);

                StartReceive();
                QueuePacket(new HandshakePacket(User.Username));
            }
            else
            {
                throw new Exception("Could not connect to server!");
            }
        }

        public void Disconnect()
        {
            if (!Connected)
                return;

            QueuePacket(new DisconnectPacket("Disconnecting"));
            
            Interlocked.CompareExchange(ref connected, 0, 1);
        }

        public void SendMessage(string message)
        {
            var parts = message.Split('\n');
            foreach (var part in parts)
                QueuePacket(new ChatMessagePacket(part));
        }

        public void QueuePacket(IPacket packet)
        {
            if (!Connected || (Client != null && !Client.Connected))
                return;

            using (MemoryStream writeStream = new MemoryStream())
            {
                using (MinecraftStream ms = new MinecraftStream(writeStream))
                {
                    ms.WriteUInt8(packet.ID);
                    packet.WritePacket(ms);
                }

                byte[] buffer = writeStream.ToArray();

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.UserToken = packet;
                args.Completed += OperationCompleted;
                args.SetBuffer(buffer, 0, buffer.Length);

                if (Client != null && !Client.Client.SendAsync(args))
                    OperationCompleted(this, args);
            }
        }

        private void StartReceive()
        {
            SocketAsyncEventArgs args = SocketPool.Get();
            args.Completed += OperationCompleted;

            if (!Client.Client.ReceiveAsync(args))
                OperationCompleted(this, args);
        }

        private void OperationCompleted(object sender, SocketAsyncEventArgs e)
        {
            e.Completed -= OperationCompleted;

            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessNetwork(e);

                    SocketPool.Add(e);
                    break;
                case SocketAsyncOperation.Send:
                    IPacket packet = e.UserToken as IPacket;

                    if (packet is DisconnectPacket)
                    {
                        Client.Client.Shutdown(SocketShutdown.Send);
                        Client.Close();

                        cancel.Cancel();
                    }

                    e.SetBuffer(null, 0, 0);
                    break;
            }
        }

        private void ProcessNetwork(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
            {
                SocketAsyncEventArgs newArgs = SocketPool.Get();
                newArgs.Completed += OperationCompleted;

                if (Client != null && !Client.Client.ReceiveAsync(newArgs))
                    OperationCompleted(this, newArgs);

                try
                {
                    sem.Wait(cancel.Token);
                }
                catch (OperationCanceledException)
                {
                    return;
                }

                var packets = PacketReader.ReadPackets(this, e.Buffer, e.Offset, e.BytesTransferred, false);

                foreach (IPacket packet in packets)
                {
                    if (PacketHandlers[packet.ID] != null)
                        PacketHandlers[packet.ID](packet, this);
                }
                
                if (sem != null)
                    sem.Release();
            }
            else
            {
                Disconnect();
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

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disconnect();

                sem.Dispose();
            }

            sem = null;
        }

        ~MultiplayerClient()
        {
            Dispose(false);
        }
    }
}