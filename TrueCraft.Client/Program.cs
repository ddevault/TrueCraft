using System;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using TrueCraft.Core;
using System.Threading;
using System.Reflection;

namespace TrueCraft.Client
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += AppDomain_CurrentDomain_AssemblyResolve;

            var thread = new Thread(() => Main_Thread(args));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }

        static Assembly AppDomain_CurrentDomain_AssemblyResolve (object sender, ResolveEventArgs args)
        {
            var assemblyName = new AssemblyName(args.Name);
            if (assemblyName.Name != "MonoGame.Framework")
                return null;
            if (RuntimeInfo.IsLinux)
                return Assembly.LoadFile("MonoGame.Framework.Linux.dll");
            if (RuntimeInfo.IsWindows)
                return Assembly.LoadFile("MonoGame.Framework.Windows.dll");
            // TODO: OSX support
            return null;
        }

        // We need to spawn the main thread manually so we can register the assembly resolver
        // and manage apartment state ourselves.
        private static void Main_Thread(string[] args)
        {
            UserSettings.Local = new UserSettings();
            UserSettings.Local.Load();

            var user = new TrueCraftUser { Username = args[1] };
            var client = new MultiplayerClient(user);
            var game = new TrueCraftGame(client, ParseEndPoint(args[0]));
            game.Run();
            client.Disconnect();
        }

        private static IPEndPoint ParseEndPoint(string arg)
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
            return new IPEndPoint(Resolve(arg), 25565);
        }

        private static IPAddress Resolve(string arg)
        {
            return Dns.GetHostEntry(arg).AddressList.FirstOrDefault(item => item.AddressFamily == AddressFamily.InterNetwork);
        }
    }
}
