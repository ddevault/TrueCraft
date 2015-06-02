using System;
using Xwt;
using System.Collections.Generic;
using System.Diagnostics;

namespace TrueCraft.Launcher.Views
{
    public class MultiplayerView : VBox
    {
        public LauncherWindow Window { get; set; }

        public Label MultiplayerLabel { get; set; }
        public TextEntry ServerIPEntry { get; set; }
        public Button ConnectButton { get; set; }
        public Button BackButton { get; set; }

        public MultiplayerView(LauncherWindow window)
        {
            Window = window;
            this.MinWidth = 250;

            MultiplayerLabel = new Label("Multiplayer")
            {
                Font = Font.WithSize(16),
                TextAlignment = Alignment.Center
            };
            ServerIPEntry = new TextEntry()
            {
                PlaceholderText = "Server IP"
            };
            ConnectButton = new Button("Connect");
            BackButton = new Button("Back");

            BackButton.Clicked += (sender, e) => 
            {
                Window.MainContainer.Remove(this);
                Window.MainContainer.PackEnd(Window.MainMenuView);
            };
            ConnectButton.Clicked += ConnectButton_Clicked;

            this.PackEnd(BackButton);
            this.PackEnd(ConnectButton);
            this.PackStart(MultiplayerLabel);
            this.PackStart(ServerIPEntry);
        }

        void ConnectButton_Clicked(object sender, EventArgs e)
        {
            string TrueCraftLaunchParams = string.Format("{0} {1} {2}", ServerIPEntry.Text, Window.User.Username, Window.User.SessionId);
            var process = new Process();
            if (RuntimeInfo.IsMono)
                process.StartInfo = new ProcessStartInfo("mono", "TrueCraft.Client.exe " + TrueCraftLaunchParams);
            else
                process.StartInfo = new ProcessStartInfo("TrueCraft.Client.exe", TrueCraftLaunchParams);
            process.EnableRaisingEvents = true;
            process.Exited += (s, a) => Application.Invoke(ClientExited);
            process.Start();
            Window.ShowInTaskbar = false;
            Window.Hide();
        }

        void ClientExited()
        {
            Window.Show();
            Window.ShowInTaskbar = true;
        }
    }
}