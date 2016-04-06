using System;
using Xwt;
using System.Threading;
using System.Net;
using TrueCraft.Core;

namespace TrueCraft.Launcher
{
    class Program
    {
        public static LauncherWindow Window { get; set; }

        [STAThread]
        public static void Main(string[] args)
        {
            if (RuntimeInfo.IsLinux)
                Application.Initialize(ToolkitType.Gtk);
            else if (RuntimeInfo.IsMacOSX)
                Application.Initialize(ToolkitType.Gtk); // TODO: Cocoa
            else if (RuntimeInfo.IsWindows)
                Application.Initialize(ToolkitType.Wpf);
            UserSettings.Local = new UserSettings();
            UserSettings.Local.Load();
            var thread = new Thread(KeepSessionAlive);
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.Lowest;
            Window = new LauncherWindow();
            thread.Start();
            Window.Show();
            Window.Closed += (sender, e) => Application.Exit();
            Application.Run();
            Window.Dispose();
            thread.Abort();
        }

        private static void KeepSessionAlive()
        {
            while (true)
            {
                if (!string.IsNullOrEmpty(Window.User.SessionId))
                {
                    var wc = new WebClient();
                    wc.DownloadString(string.Format(TrueCraftUser.AuthServer + "/session?name={0}&session={1}",
                        Window.User.Username, Window.User.SessionId));
                }
                Thread.Sleep(60 * 5 * 1000);
            }
        }
    }
}
