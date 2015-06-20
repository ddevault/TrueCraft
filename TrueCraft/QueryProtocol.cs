using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using TrueCraft.API.Server;

namespace TrueCraft
{
    public class QueryProtocol
    {
        private UdpClient Udp;
        private int Port;
        private Timer Timer;
        private Random Rnd;
        private IMultiplayerServer Server;
        private CancellationTokenSource CToken;

        private readonly byte[] ProtocolVersion = new byte[] { 0xFE, 0xFD };
        private readonly byte Type_Handshake = 0x09;
        private readonly byte Type_Stat = 0x00;

        private Dictionary<IPEndPoint, QueryUser> UserList;
        private object UserLock = new object();

        public QueryProtocol(IMultiplayerServer server)
        {
            Rnd = new Random();
            Server = server;
        }
        public void Start()
        {
            Port = Program.ServerConfiguration.QueryPort;
            Udp = new UdpClient(Port);
            Timer = new Timer(ResetUserList, null, 0, 30000);
            CToken = new CancellationTokenSource();
            Udp.BeginReceive(HandleReceive, null);
        }

        private void HandleReceive(IAsyncResult ar)
        {
            if (CToken.IsCancellationRequested) return;

            try
            {
                var clientEP = new IPEndPoint(IPAddress.Any, Port);
                byte[] buffer = Udp.EndReceive(ar, ref clientEP);

                if (CheckVersion(buffer))
                {
                    if (buffer[2] == Type_Handshake)
                        HandleHandshake(buffer, clientEP);
                    else if (buffer[2] == Type_Stat)
                    {
                        if (buffer.Length == 11)
                            HandleBasicStat(buffer, clientEP);
                        else if (buffer.Length == 15)
                            HandleFullStat(buffer, clientEP);
                    }
                }
            }
            catch { }
            if (CToken.IsCancellationRequested) return;

            Udp.BeginReceive(HandleReceive, null);
        }

        private void HandleHandshake(byte[] buffer, IPEndPoint clientEP)
        {
            var stream = GetStream(buffer);
            int sessionId = GetSessionId(stream);

            var user = new QueryUser { SessionId = sessionId, ChallengeToken = Rnd.Next() };
            lock (UserLock)
            {
                if (UserList.ContainsKey(clientEP))
                    UserList.Remove(clientEP);
                UserList.Add(clientEP, user);
            }

            var response = GetStream();
            WriteHead(Type_Handshake, user, response);
            WriteStringToStream(user.ChallengeToken.ToString(), response.BaseStream);

            SendResponse(response, clientEP);
        }

        private void HandleBasicStat(byte[] buffer, IPEndPoint clientEP)
        {
            var stream = GetStream(buffer);
            int sessionId = GetSessionId(stream);
            int token = GetToken(stream);

            var user = GetUser(clientEP);
            if (user.ChallengeToken != token || user.SessionId != sessionId) throw new Exception("Invalid credentials");

            var stats = GetStats();
            var response = GetStream();
            WriteHead(Type_Stat, user, response);
            WriteStringToStream(stats["hostname"], response.BaseStream);
            WriteStringToStream(stats["gametype"], response.BaseStream);
            WriteStringToStream(stats["numplayers"], response.BaseStream);
            WriteStringToStream(stats["maxplayers"], response.BaseStream);
            byte[] hostport = BitConverter.GetBytes(UInt16.Parse(stats["hostport"]));
            Array.Reverse(hostport);//The specification needs little endian short
            response.Write(hostport);
            WriteStringToStream(stats["hostip"], response.BaseStream);

            SendResponse(response, clientEP);
        }

        private void HandleFullStat(byte[] buffer, IPEndPoint clientEP)
        {
            var stream = GetStream(buffer);
            int sessionId = GetSessionId(stream);
            int token = GetToken(stream);

            var user = GetUser(clientEP);
            if (user.ChallengeToken != token || user.SessionId != sessionId) throw new Exception("Invalid credentials");

            var stats = GetStats();
            var response = GetStream();
            WriteHead(Type_Stat, user, response);
            WriteStringToStream("SPLITNUM", response.BaseStream);
            foreach (var pair in stats)
            {
                WriteStringToStream(pair.Key, response.BaseStream);
                WriteStringToStream(pair.Value, response.BaseStream);
            }
            response.Write((byte)0x01);
            WriteStringToStream("player_\0", response.BaseStream);
            var players = GetPlayers();
            foreach (string player in players)
                WriteStringToStream(player, response.BaseStream);
            response.Write((byte)0x00);

            SendResponse(response, clientEP);
        }

        private bool CheckVersion(byte[] ver)
        {
            return ver[0] == ProtocolVersion[0] && ver[1] == ProtocolVersion[1];
        }
        private int GetSessionId(BinaryReader stream)
        {
            stream.BaseStream.Position = 3;
            return stream.ReadInt32();
        }
        private int GetToken(BinaryReader stream)
        {
            stream.BaseStream.Position = 7;
            return stream.ReadInt32();
        }
        private BinaryReader GetStream(byte[] buffer)
        { return new BinaryReader(new MemoryStream(buffer)); }
        private BinaryWriter GetStream()
        { return new BinaryWriter(new MemoryStream()); }
        private void WriteHead(byte type, QueryUser user, BinaryWriter stream)
        {
            stream.Write(type);
            stream.Write(user.SessionId);
        }
        private void SendResponse(BinaryWriter res, IPEndPoint destination)
        {
            byte[] data = ((MemoryStream)res.BaseStream).ToArray();
            Udp.Send(data, data.Length, destination);
        }
        private QueryUser GetUser(IPEndPoint ep)
        {
            QueryUser user;
            lock (UserLock)
                if (!UserList.TryGetValue(ep, out user)) throw new Exception("Undefined user");
            return user;
        }
        private Dictionary<string, string> GetStats()
        {
            var stats = new Dictionary<string, string>();
            stats.Add("hostname", Program.ServerConfiguration.MOTD);
            stats.Add("gametype", "SMP");
            stats.Add("game_id", "TRUECRAFT");
            stats.Add("version", "1.0");
            stats.Add("plugins", "");
            stats.Add("map", Server.Worlds.First().Name);
            stats.Add("numplayers", Server.Clients.Count.ToString());
            stats.Add("maxplayers", "64");
            stats.Add("hostport", Program.ServerConfiguration.ServerPort.ToString());
            stats.Add("hostip", Program.ServerConfiguration.ServerAddress);
            return stats;
        }
        private List<string> GetPlayers()
        {
            var names = new List<string>();
            lock (Program.Server.ClientLock)
                foreach (var client in Server.Clients)
                    names.Add(client.Username);
            return names;
        }

        public void Stop()
        {
            CToken.Cancel();
            Udp.Close();
        }

        private void ResetUserList(object state)
        {
            lock (UserLock)
                UserList = new Dictionary<IPEndPoint, QueryUser>();
        }

        struct QueryUser
        {
            public int SessionId;
            public int ChallengeToken;
        }

        private byte[] String0ToBytes(string s)
        { return Encoding.UTF8.GetBytes(s + "\0"); }
        private void WriteToStream(byte[] bytes, Stream stream)
        { stream.Write(bytes, 0, bytes.Length); }
        private void WriteStringToStream(string s, Stream stream)
        { WriteToStream(String0ToBytes(s), stream); }
    }
}
