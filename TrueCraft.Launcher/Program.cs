using System;
using Xwt;

namespace TrueCraft.Launcher
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            if (RuntimeInfo.IsLinux)
                Application.Initialize(ToolkitType.Gtk);
            else if (RuntimeInfo.IsMacOSX)
                Application.Initialize(ToolkitType.Gtk);
            else if (RuntimeInfo.IsWindows)
                Application.Initialize(ToolkitType.Wpf);
            var window = new LauncherWindow();
            window.Show();
            window.Closed += (sender, e) => Application.Exit();
            Application.Run();
            window.Dispose();
        }
    }
}
