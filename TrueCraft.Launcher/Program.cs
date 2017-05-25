using System;
using Xwt;
using System.Threading;
using System.Net;
using TrueCraft.Core;
using System.Runtime.InteropServices;

namespace TrueCraft.Launcher
{
    class Program
    {
        [DllImport("libgtk-3.so.0", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gtk_get_major_version();

        public static LauncherWindow Window { get; set; }

        [STAThread]
        public static void Main(string[] args)
        {
            if (RuntimeInfo.IsLinux)
            {
                try
                {
                    // Call the function to see if the GTK3
                    // library can be loaded.
                    gtk_get_major_version();

                    Application.Initialize(ToolkitType.Gtk3);
                }
                catch
                {
                    Application.Initialize (ToolkitType.Gtk);
                }
            }
            else if (RuntimeInfo.IsMacOSX)
                Application.Initialize(ToolkitType.Gtk); // TODO: Cocoa
            else if (RuntimeInfo.IsWindows)
                Application.Initialize(ToolkitType.Wpf);
            else
                // In this case they're probably using some flavor of Unix
                // which probably has some flavor of GTK availble
                Application.Initialize(ToolkitType.Gtk);
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
