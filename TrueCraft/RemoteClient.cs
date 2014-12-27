using System;
using TrueCraft.API.Networking;
using System.Net.Sockets;
using TrueCraft.Core.Networking;
using System.Collections.Concurrent;

namespace TrueCraft
{
    public class RemoteClient : IRemoteClient
    {
        public RemoteClient(NetworkStream stream)
        {
            NetworkStream = stream;
            MinecraftStream = new MinecraftStream(NetworkStream);
            PacketQueue = new ConcurrentQueue<IPacket>();
        }
        
        public NetworkStream NetworkStream { get; set; }
        public IMinecraftStream MinecraftStream { get; internal set; }
        public ConcurrentQueue<IPacket> PacketQueue { get; private set; }
        public string Username { get; internal set; }

        public bool DataAvailable
        {
            get
            {
                return NetworkStream.DataAvailable;
            }
        }

        public void QueuePacket(IPacket packet)
        {
            PacketQueue.Enqueue(packet);
        }
    }
}