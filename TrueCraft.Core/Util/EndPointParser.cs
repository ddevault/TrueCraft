using System.Net;
using System.Linq;
using System.Net.Sockets;

namespace TrueCraft.Core.Util
{
    public class EndPointParser
    {
        public static IPEndPoint ParseEndPoint(string arg)
        {
            IPAddress address;
            int port;
            if (arg.Contains(':'))
            {
                // Both IP and port are specified
                var parts = arg.Split(':');
                if (!IPAddress.TryParse(parts[0], out address))
                    address = Resolve(parts[0]);
                return new IPEndPoint(address, int.Parse(parts[1]));
            }
            if (IPAddress.TryParse(arg, out address))
                return new IPEndPoint(address, 25565);
            if (int.TryParse(arg, out port))
                return new IPEndPoint(IPAddress.Loopback, port);
            var ipAddress = Resolve(arg);
            return ipAddress == null ? null : new IPEndPoint(ipAddress, 25565);
        }

        private static IPAddress Resolve(string arg)
        {
            try
            {
                return
                    Dns.GetHostEntry(arg)
                        .AddressList.FirstOrDefault(item => item.AddressFamily == AddressFamily.InterNetwork);
            }
            catch (SocketException)
            {
                return null;
            }
        }
    }
}
