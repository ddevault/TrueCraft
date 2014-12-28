using System;
using TrueCraft.API.Server;
using TrueCraft.API.Networking;
using TrueCraft.Core.Networking;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using TrueCraft.API.World;
using TrueCraft.API.Logging;
using TrueCraft.Core.Networking.Packets;

namespace TrueCraft
{
    public class MultiplayerServer : IMultiplayerServer
    {
        public IPacketReader PacketReader { get; private set; }
        public IList<IRemoteClient> Clients { get; private set; }
        public IList<IWorld> Worlds { get; private set; }
        public IList<IEntityManager> EntityManagers { get; private set; }
        public IEventScheduler Scheduler { get; private set; }

        private Timer NetworkWorker, EnvironmentWorker;
        private TcpListener Listener;
        private readonly PacketHandler[] PacketHandlers;
        private IList<ILogProvider> LogProviders;

        public MultiplayerServer()
        {
            var reader = new PacketReader();
            PacketReader = reader;
            Clients = new List<IRemoteClient>();
            NetworkWorker = new Timer(DoNetwork);
            EnvironmentWorker = new Timer(DoEnvironment);
            PacketHandlers = new PacketHandler[0x100];
            Worlds = new List<IWorld>();
            EntityManagers = new List<IEntityManager>();
            LogProviders = new List<ILogProvider>();
            Scheduler = new EventScheduler(this);

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
            Log(LogCategory.Notice, "Running TrueCraft server on {0}", endPoint);
            NetworkWorker.Change(100, 1000 / 20);
            EnvironmentWorker.Change(100, 1000 / 20);
        }

        public void AddWorld(IWorld world)
        {
            Worlds.Add(world);
            var manager = new EntityManager(this, world);
            EntityManagers.Add(manager);
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

        private void DisconnectClient(IRemoteClient _client)
        {
            var client = (RemoteClient)_client;
            if (client.LoggedIn)
                Log(LogCategory.Notice, "{0} has left the server.", client.Username);
            Clients.Remove(client);
        }

        private void AcceptClient(IAsyncResult result)
        {
            var tcpClient = Listener.EndAcceptTcpClient(result);
            var client = new RemoteClient(this, tcpClient.GetStream());
            Clients.Add(client);
            Listener.BeginAcceptTcpClient(AcceptClient, null);
        }

        private void DoEnvironment(object discarded)
        {
            Scheduler.Update();
        }

        private void DoNetwork(object discarded)
        {
            for (int i = 0; i < Clients.Count; i++)
            {
                var client = Clients[i] as RemoteClient;
                if (client.PacketQueue.Count != 0)
                {
                    IPacket packet;
                    while (!client.PacketQueue.TryDequeue(out packet)) { }
                    PacketReader.WritePacket(client.MinecraftStream, packet);
                    if (packet is DisconnectPacket)
                    {
                        DisconnectClient(client);
                        i--;
                    }
                }
                if (client.DataAvailable)
                {
                    var packet = PacketReader.ReadPacket(client.MinecraftStream);
                    if (PacketHandlers[packet.ID] != null)
                    {
                        try
                        {
                            PacketHandlers[packet.ID](packet, client, this);
                        }
                        catch (Exception e)
                        {
                            Log(LogCategory.Debug, "Disconnecting client due to exception in network worker");
                            Log(LogCategory.Debug, e.ToString());
                            DisconnectClient(client);
                            i--;
                        }
                    }
                    else
                    {
                        // TODO: Something productive
                    }
                }
            }
        }
    }
}