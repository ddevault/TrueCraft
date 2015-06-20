using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace TrueCraft
{
    public class QueryProtocol
    {
        private UdpClient Udp;
        private int Port;
        private Timer Timer;
        private Random Rnd;
        private CancellationTokenSource CToken;

        private readonly Tuple<byte, byte> ProtocolVersion = new Tuple<byte, byte>(0xFE, 0xFD);
        private readonly byte Type_Handshake = 0x09;
        private readonly byte Type_Stat = 0x00;

        private Dictionary<IPEndPoint, QueryUser> UserList;
        private object UserLock = new object();

        public QueryProtocol(int port)
        {
            Port = port;
            Rnd = new Random();
        }
        public void Start()
        {
            Udp = new UdpClient(Port);
            Timer = new Timer(ResetUserList, null, 0, 30000);
            CToken = new CancellationTokenSource();
            Udp.BeginReceive(HandleReceive, null);
        }

        private void HandleReceive(IAsyncResult ar)
        {
            if (CToken.IsCancellationRequested) return;

            var clientEP = new IPEndPoint(IPAddress.Any, Port);
            byte[] buffer = Udp.EndReceive(ar, ref clientEP);

            switch (buffer.Length)
            {
                case 7: HandleHandshake(buffer, clientEP); break;
                case 11: HandleBasicStat(buffer, clientEP); break;
                case 15: HandleFullStat(buffer, clientEP); break;
            }

            if (CToken.IsCancellationRequested) return;

            Udp.BeginReceive(HandleReceive, null);
        }

        private void HandleHandshake(byte[] buffer, IPEndPoint clientEP)
        {
            var stream = GetStream(buffer);
            CheckHead(Type_Handshake, stream);
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
            CheckHead(Type_Stat, stream);
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
            CheckHead(Type_Stat, stream);
            int sessionId = GetSessionId(stream);
            int token = GetToken(stream);

            var user = GetUser(clientEP);
            if (user.ChallengeToken != token || user.SessionId != sessionId) throw new Exception("Invalid credentials");

            var stats = GetStats();
            var response = GetStream();
            WriteHead(Type_Stat, user, response);
            foreach (var pair in stats)
            {
                WriteStringToStream(pair.Key, response.BaseStream);
                WriteStringToStream(pair.Value, response.BaseStream);
            }

            SendResponse(response, clientEP);
        }

        private void CheckVersion(BinaryReader stream)
        {
            byte ver1 = stream.ReadByte();
            byte ver2 = stream.ReadByte();
            if (ver1 != ProtocolVersion.Item1 || ver2 != ProtocolVersion.Item2) 
                throw new Exception("Incorrect Protocol Version");
        }
        private void CheckType(byte Type, BinaryReader stream)
        {
            byte type = stream.ReadByte();
            if (type != Type) throw new Exception("Incorrect Type");
        }
        private void CheckHead(byte Type, BinaryReader stream)
        {
            CheckVersion(stream);
            CheckType(Type, stream);
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
            stats.Add("map", Program.Server.Worlds.First().Name);
            stats.Add("numplayers", Program.Server.Clients.Count.ToString());
            stats.Add("maxplayers", "64");
            stats.Add("hostport", Program.ServerConfiguration.ServerPort.ToString());
            stats.Add("hostip", Program.ServerConfiguration.ServerAddress);
            return stats;
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
