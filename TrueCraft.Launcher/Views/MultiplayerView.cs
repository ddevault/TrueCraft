using System;
using Xwt;
using System.Collections.Generic;
using System.Diagnostics;
using Xwt.Drawing;
using System.Reflection;
using System.Linq;
using TrueCraft.Core;

namespace TrueCraft.Launcher.Views
{
    public class MultiplayerView : VBox
    {
        public LauncherWindow Window { get; set; }

        public Label MultiplayerLabel { get; set; }
        public TextEntry ServerIPEntry { get; set; }
        public Button ConnectButton { get; set; }
        public Button BackButton { get; set; }
        public Button AddServerButton { get; set; }
        public Button RemoveServerButton { get; set; }
        public ListView ServerListView { get; set; }
        public VBox ServerCreationBox { get; set; }
        public Label NewServerLabel { get; set; }
        public TextEntry NewServerName { get; set; }
        public TextEntry NewServerAddress { get; set; }
        public Button CommitAddNewServer { get; set; }
        public Button CancelAddNewServer { get; set; }
        public ListStore ServerListStore { get; set; }

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
                PlaceholderText = "Server IP",
                Text = UserSettings.Local.LastIP
            };
            ConnectButton = new Button("Connect");
            BackButton = new Button("Back");
            ServerListView = new ListView() { MinHeight = 200, SelectionMode = SelectionMode.Single };
            AddServerButton = new Button("Add server");
            RemoveServerButton = new Button("Remove") { Sensitive = false };
            ServerCreationBox = new VBox() { Visible = false };
            NewServerLabel = new Label("Add new server:") { TextAlignment = Alignment.Center };
            NewServerName = new TextEntry() { PlaceholderText = "Name" };
            NewServerAddress = new TextEntry() { PlaceholderText = "Address" };
            CommitAddNewServer = new Button("Add server");
            CancelAddNewServer = new Button("Cancel");

            var iconField = new DataField<Image>();
            var nameField = new DataField<string>();
            var playersField = new DataField<string>();
            ServerListStore = new ListStore(iconField, nameField, playersField);
            ServerListView.DataSource = ServerListStore;
            ServerListView.HeadersVisible = false;
            ServerListView.Columns.Add(new ListViewColumn("Icon", new ImageCellView { ImageField = iconField }));
            ServerListView.Columns.Add(new ListViewColumn("Name", new TextCellView { TextField = nameField }));
            ServerListView.Columns.Add(new ListViewColumn("Players", new TextCellView { TextField = playersField }));

            ServerIPEntry.KeyReleased += (sender, e) => 
            {
                if (e.Key == Key.Return || e.Key == Key.NumPadEnter)
                    ConnectButton_Clicked(sender, e);
            };
            BackButton.Clicked += (sender, e) =>
            {
                Window.InteractionBox.Remove(this);
                Window.InteractionBox.PackEnd(Window.MainMenuView);
            };
            ConnectButton.Clicked += ConnectButton_Clicked;
            ServerListView.SelectionChanged += (sender, e) => 
            {
                RemoveServerButton.Sensitive = ServerListView.SelectedRow != -1;
                ServerIPEntry.Sensitive = ServerListView.SelectedRow == -1;
            };
            AddServerButton.Clicked += (sender, e) => 
            {
                AddServerButton.Sensitive = false;
                RemoveServerButton.Sensitive = false;
                ConnectButton.Sensitive = false;
                ServerListView.Sensitive = false;
                ServerIPEntry.Sensitive = false;
                ServerCreationBox.Visible = true;
            };
            CancelAddNewServer.Clicked += (sender, e) => 
            {
                AddServerButton.Sensitive = true;
                RemoveServerButton.Sensitive = true;
                ConnectButton.Sensitive = true;
                ServerListView.Sensitive = true;
                ServerIPEntry.Sensitive = true;
                ServerCreationBox.Visible = false;
            };
            RemoveServerButton.Clicked += (sender, e) => 
            {
                var server = UserSettings.Local.FavoriteServers[ServerListView.SelectedRow];
                ServerListStore.RemoveRow(ServerListView.SelectedRow);
                UserSettings.Local.FavoriteServers = UserSettings.Local.FavoriteServers.Where(
                    s => s.Name != server.Name && s.Address != server.Address).ToArray();
                UserSettings.Local.Save();
            };
            CommitAddNewServer.Clicked += (sender, e) => 
            {
                var server = new FavoriteServer
                {
                    Name = NewServerName.Text,
                    Address = NewServerAddress.Text
                };
                var row = ServerListStore.AddRow();
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TrueCraft.Launcher.Content.default-server-icon.png"))
                    ServerListStore.SetValue(row, iconField, Image.FromStream(stream));
                ServerListStore.SetValue(row, nameField, server.Name);
                ServerListStore.SetValue(row, playersField, "TODO/50");
                UserSettings.Local.FavoriteServers = UserSettings.Local.FavoriteServers.Concat(new[] { server }).ToArray();
                UserSettings.Local.Save();
                AddServerButton.Sensitive = true;
                RemoveServerButton.Sensitive = true;
                ConnectButton.Sensitive = true;
                ServerListView.Sensitive = true;
                ServerIPEntry.Sensitive = true;
                ServerCreationBox.Visible = false;
            };

            foreach (var server in UserSettings.Local.FavoriteServers)
            {
                var row = ServerListStore.AddRow();
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TrueCraft.Launcher.Content.default-server-icon.png"))
                    ServerListStore.SetValue(row, iconField, Image.FromStream(stream));
                ServerListStore.SetValue(row, nameField, server.Name);
                ServerListStore.SetValue(row, playersField, "TODO/50");
            }

            var addServerHBox = new HBox();
            AddServerButton.WidthRequest = RemoveServerButton.WidthRequest = 0.5;
            addServerHBox.PackStart(AddServerButton, true);
            addServerHBox.PackStart(RemoveServerButton, true);

            var commitHBox = new HBox();
            CancelAddNewServer.WidthRequest = CommitAddNewServer.WidthRequest = 0.5;
            commitHBox.PackStart(CommitAddNewServer, true);
            commitHBox.PackStart(CancelAddNewServer, true);

            ServerCreationBox.PackStart(NewServerLabel);
            ServerCreationBox.PackStart(NewServerName);
            ServerCreationBox.PackStart(NewServerAddress);
            ServerCreationBox.PackStart(commitHBox);

            this.PackEnd(BackButton);
            this.PackEnd(ConnectButton);
            this.PackStart(MultiplayerLabel);
            this.PackStart(ServerIPEntry);
            this.PackStart(ServerListView);
            this.PackStart(addServerHBox);
            this.PackStart(ServerCreationBox);
        }

        void ConnectButton_Clicked(object sender, EventArgs e)
        {
            var ip = ServerIPEntry.Text;
            if (ServerListView.SelectedRow != -1)
                ip = UserSettings.Local.FavoriteServers[ServerListView.SelectedRow].Address;
            string TrueCraftLaunchParams = string.Format("{0} {1} {2}", ip, Window.User.Username, Window.User.SessionId);
            var process = new Process();
            if (RuntimeInfo.IsMono)
                process.StartInfo = new ProcessStartInfo("mono", "TrueCraft.Client.exe " + TrueCraftLaunchParams);
            else
                process.StartInfo = new ProcessStartInfo("TrueCraft.Client.exe", TrueCraftLaunchParams);
            process.EnableRaisingEvents = true;
            process.Exited += (s, a) => Application.Invoke(ClientExited);
            process.Start();
            UserSettings.Local.LastIP = ServerIPEntry.Text;
            UserSettings.Local.Save();
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