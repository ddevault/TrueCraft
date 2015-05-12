using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;
using TrueCraft.API.Networking;
using System.Threading;
using TrueCraft.Core.Networking;
using System.Linq;
using TrueCraft.Core.Networking.Packets;

// TODO: Make IMultiplayerClient and so on
namespace TrueCraft.Client.Linux
{
    public delegate void PacketHandler(IPacket packet, MultiplayerClient client);

    public class MultiplayerClient
    {
        private TcpClient Client { get; set; }
        private IMinecraftStream Stream { get; set; }
        private PacketReader PacketReader { get; set; }
        private ConcurrentQueue<IPacket> PacketQueue { get; set; }
        private Thread NetworkWorker { get; set; }
        private readonly PacketHandler[] PacketHandlers;

        public MultiplayerClient()
        {
            Client = new TcpClient();
            PacketQueue = new ConcurrentQueue<IPacket>();
            PacketReader = new PacketReader();
            PacketReader.RegisterCorePackets();
            NetworkWorker = new Thread(new ThreadStart(DoNetwork));
            PacketHandlers = new PacketHandler[0x100];
            Handlers.PacketHandlers.RegisterHandlers(this);
        }

        public void RegisterPacketHandler(byte packetId, PacketHandler handler)
        {
            PacketHandlers[packetId] = handler;
        }

        public void Connect(IPEndPoint endPoint)
        {
            Client.BeginConnect(endPoint.Address, endPoint.Port, ConnectionComplete, null);
        }

        public void QueuePacket(IPacket packet)
        {
            PacketQueue.Enqueue(packet);
        }

        private void ConnectionComplete(IAsyncResult result)
        {
            Client.EndConnect(result);
            Stream = new MinecraftStream(new BufferedStream(Client.GetStream()));
            NetworkWorker.Start();
            QueuePacket(new HandshakePacket("TestUser")); // TODO: Get username from somewhere else
        }

        private void DoNetwork()
        {
            bool idle = true;
            while (true)
            {
                IPacket packet;
                while (Client.Available != 0)
                {
                    idle = false;
                    packet = PacketReader.ReadPacket(Stream, false);
                    if (PacketHandlers[packet.ID] != null)
                        PacketHandlers[packet.ID](packet, this);
                }
                while (PacketQueue.Any())
                {
                    idle = false;
                    while (!PacketQueue.TryDequeue(out packet)) { }
                    PacketReader.WritePacket(Stream, packet);
                    Stream.BaseStream.Flush();
                }
                if (idle)
                    Thread.Sleep(100);
            }
        }
    }
}