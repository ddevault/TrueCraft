using System;
using TrueCraft.API.Server;
using TrueCraft.API.Networking;
using TrueCraft.Core.Networking;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;

namespace TrueCraft
{
    public class MultiplayerServer : IMultiplayerServer
    {
        public IPacketReader PacketReader { get; private set; }
        public IList<IRemoteClient> Clients { get; private set; }

        private Timer NetworkWorker;
        private TcpListener Listener;
        private PacketHandler[] PacketHandlers;

        public MultiplayerServer()
        {
            var reader = new PacketReader();
            PacketReader = reader;
            Clients = new List<IRemoteClient>();
            NetworkWorker = new Timer(DoNetwork);
            PacketHandlers = new PacketHandler[0x100];

            reader.RegisterCorePackets();
            Handlers.PacketHandlers.RegisterHandlers(this);
        }

        public void RegisterPacketHandler(byte packetId, PacketHandler handler)
        {
            PacketHandlers[packetId] = handler;
        }

        public void Start(IPEndPoint endPoint)
        {
            Listener = new TcpListener(endPoint);
            Listener.Start();
            Listener.BeginAcceptTcpClient(AcceptClient, null);
            NetworkWorker.Change(100, 1000 / 20);
        }

        private void AcceptClient(IAsyncResult result)
        {
            var tcpClient = Listener.EndAcceptTcpClient(result);
            var client = new RemoteClient(tcpClient.GetStream());
            Clients.Add(client);
        }

        private void DoNetwork(object discarded)
        {
            for (int i = 0, ClientsCount = Clients.Count; i < ClientsCount; i++)
            {
                var client = Clients[i] as RemoteClient;
                if (client.PacketQueue.Count != 0)
                {
                    IPacket packet;
                    while (!client.PacketQueue.TryDequeue(out packet)) { }
                    Console.WriteLine("Sending packet 0x{0:X2}: {1}", packet.ID, packet.GetType().Name);
                    PacketReader.WritePacket(client.MinecraftStream, packet);
                }
                if (client.DataAvailable)
                {
                    var packet = PacketReader.ReadPacket(client.MinecraftStream);
                    if (PacketHandlers[packet.ID] != null)
                    {
                        Console.WriteLine("Got packet 0x{0:X2}: {1}", packet.ID, packet.GetType().Name);
                        try
                        {
                            PacketHandlers[packet.ID](packet, client, this);
                        }
                        catch (Exception e)
                        {
                            // TODO: Something else
                            Clients.Remove(client);
                            i--;
                        }
                    }
                    else
                    {
                        Console.WriteLine("WARNING: Got unhandled packet 0x{0:X2}: {1}", packet.ID, packet.GetType().Name);
                    }
                }
            }
        }
    }
}