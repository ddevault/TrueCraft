using System;
using Xwt;
using TrueCraft.Launcher.Singleplayer;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using System.IO;
using TrueCraft.Core;

namespace TrueCraft.Launcher.Views
{
    public class SingleplayerView : VBox
    {
        public LauncherWindow Window { get; set; }
        public Label SingleplayerLabel { get; set; }
        public ListView WorldListView { get; set; }
        public Button CreateWorldButton { get; set; }
        public Button DeleteWorldButton { get; set; }
        public Button PlayButton { get; set; }
        public Button BackButton { get; set; }
        public VBox CreateWorldBox { get; set; }
        public TextEntry NewWorldName { get; set; }
        public TextEntry NewWorldSeed { get; set; }
        public Button NewWorldCommit { get; set; }
        public Button NewWorldCancel { get; set; }
        public ListStore WorldListStore { get; set; }
        public Label ProgressLabel { get; set; }
        public ProgressBar ProgressBar { get; set; }
        public SingleplayerServer Server { get; set; }
        public DataField<string> NameField { get; set; }

        public SingleplayerView(LauncherWindow window)
        {
            Worlds.Local = new Worlds();
            Worlds.Local.Load();

            Window = window;
            this.MinWidth = 250;

            SingleplayerLabel = new Label("Singleplayer")
            {
                Font = Font.WithSize(16),
                TextAlignment = Alignment.Center
            };
            WorldListView = new ListView
            {
                MinHeight = 200,
                SelectionMode = SelectionMode.Single
            };
            CreateWorldButton = new Button("New world");
            DeleteWorldButton = new Button("Delete") { Sensitive = false };
            PlayButton = new Button("Play") { Sensitive = false };
            BackButton = new Button("Back");
            CreateWorldBox = new VBox() { Visible = false };
            NewWorldName = new TextEntry() { PlaceholderText = "Name" };
            NewWorldSeed = new TextEntry() { PlaceholderText = "Seed (optional)" };
            NewWorldCommit = new Button("Create") { Sensitive = false };
            NewWorldCancel = new Button("Cancel");
            NameField = new DataField<string>();
            WorldListStore = new ListStore(NameField);
            WorldListView.DataSource = WorldListStore;
            WorldListView.HeadersVisible = false;
            WorldListView.Columns.Add(new ListViewColumn("Name", new TextCellView { TextField = NameField, Editable = false }));
            ProgressLabel = new Label("Loading world...") { Visible = false };
            ProgressBar = new ProgressBar() { Visible = false, Indeterminate = true, Fraction = 0 };

            BackButton.Clicked += (sender, e) =>
            {
                Window.InteractionBox.Remove(this);
                Window.InteractionBox.PackEnd(Window.MainMenuView);
            };
            CreateWorldButton.Clicked += (sender, e) =>
            {
                CreateWorldBox.Visible = true;
            };
            NewWorldCancel.Clicked += (sender, e) =>
            {
                CreateWorldBox.Visible = false;
            };
            NewWorldName.Changed += (sender, e) =>
            {
                NewWorldCommit.Sensitive = !string.IsNullOrEmpty(NewWorldName.Text);
            };
            NewWorldCommit.Clicked += NewWorldCommit_Clicked;
            WorldListView.SelectionChanged += (sender, e) => 
            {
                PlayButton.Sensitive = DeleteWorldButton.Sensitive = WorldListView.SelectedRow != -1;
            };
            PlayButton.Clicked += PlayButton_Clicked;
            DeleteWorldButton.Clicked += (sender, e) => 
            {
                var world = Worlds.Local.Saves[WorldListView.SelectedRow];
                WorldListStore.RemoveRow(WorldListView.SelectedRow);
                Worlds.Local.Saves = Worlds.Local.Saves.Where(s => s != world).ToArray();
                Directory.Delete(world.BaseDirectory, true);
            };

            foreach (var world in Worlds.Local.Saves)
            {
                var row = WorldListStore.AddRow();
                WorldListStore.SetValue(row, NameField, world.Name);
            }

            var createDeleteHbox = new HBox();
            CreateWorldButton.WidthRequest = DeleteWorldButton.WidthRequest = 0.5;
            createDeleteHbox.PackStart(CreateWorldButton, true);
            createDeleteHbox.PackStart(DeleteWorldButton, true);

            CreateWorldBox.PackStart(NewWorldName);
            CreateWorldBox.PackStart(NewWorldSeed);
            var newWorldHbox = new HBox();
            NewWorldCommit.WidthRequest = NewWorldCancel.WidthRequest = 0.5;
            newWorldHbox.PackStart(NewWorldCommit, true);
            newWorldHbox.PackStart(NewWorldCancel, true);
            CreateWorldBox.PackStart(newWorldHbox);

            this.PackStart(SingleplayerLabel);
            this.PackStart(WorldListView);
            this.PackStart(createDeleteHbox);
            this.PackStart(PlayButton);
            this.PackStart(CreateWorldBox);
            this.PackStart(ProgressLabel);
            this.PackStart(ProgressBar);
            this.PackEnd(BackButton);
        }

        public void PlayButton_Clicked(object sender, EventArgs e)
        {
            Server = new SingleplayerServer(Worlds.Local.Saves[WorldListView.SelectedRow]);
            PlayButton.Sensitive = BackButton.Sensitive = CreateWorldButton.Sensitive =
                CreateWorldBox.Visible = false;
            ProgressBar.Visible = ProgressLabel.Visible = true;
            Task.Factory.StartNew(() =>
            {
                Server.Initialize((value, stage) =>
                    Application.Invoke(() =>
                    {
                        ProgressBar.Indeterminate = false;
                        ProgressLabel.Text = stage;
                        ProgressBar.Fraction = value;
                    }));
                Server.Start();
                Application.Invoke(() =>
                {
                    PlayButton.Sensitive = BackButton.Sensitive = CreateWorldButton.Sensitive = true;
                    var launchParams = string.Format("{0} {1} {2}", Server.Server.EndPoint, Window.User.Username, Window.User.SessionId);
                    var process = new Process();
                    if (RuntimeInfo.IsMono)
                        process.StartInfo = new ProcessStartInfo("mono", "TrueCraft.Client.exe " + launchParams);
                    else
                        process.StartInfo = new ProcessStartInfo("TrueCraft.Client.exe", launchParams);
                    process.EnableRaisingEvents = true;
                    process.Exited += (s, a) => Application.Invoke(() =>
                    {
                        Server.Stop();
                        Server.World.Save();
                        ProgressBar.Visible = ProgressLabel.Visible = false;
                        Window.Show();
                        Window.ShowInTaskbar = true;
                    });
                    process.Start();
                    Window.ShowInTaskbar = false;
                    Window.Hide();
                });
            });
        }

        void NewWorldCommit_Clicked(object sender, EventArgs e)
        {
            var world = Worlds.Local.CreateNewWorld(NewWorldName.Text, NewWorldSeed.Text);
            CreateWorldBox.Visible = false;
            var row = WorldListStore.AddRow();
            WorldListStore.SetValue(row, NameField, world.Name);
        }
    }
}