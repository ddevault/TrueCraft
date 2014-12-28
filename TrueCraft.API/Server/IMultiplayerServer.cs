using System;
using TrueCraft.API.Networking;
using System.Net;
using System.Collections.Generic;
using TrueCraft.API.World;
using TrueCraft.API.Logging;

namespace TrueCraft.API.Server
{
    /// <summary>
    /// Called when the given packet comes in from a remote client. Return false to cease communication
    /// with that client.
    /// </summary>
    public delegate void PacketHandler(IPacket packet, IRemoteClient client, IMultiplayerServer server);

    public interface IMultiplayerServer
    {
        IPacketReader PacketReader { get; }
        IList<IRemoteClient> Clients { get; }
        IList<IWorld> Worlds { get; }
        IEventScheduler Scheduler { get; }

        void Start(IPEndPoint endPoint);
        void RegisterPacketHandler(byte packetId, PacketHandler handler);
        void AddWorld(IWorld world);
        void AddLogProvider(ILogProvider provider);
        void Log(LogCategory category, string text, params object[] parameters);
        IEntityManager GetEntityManagerForWorld(IWorld world);
    }
}