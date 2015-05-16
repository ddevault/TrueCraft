using System;
using Xwt;
using System.Diagnostics;
using Xwt.Drawing;

namespace TrueCraft.Launcher
{
    public class LauncherWindow : Window
    {
        public HBox MainContainer { get; set; }
        public ScrollView WebScrollView { get; set; }
        public WebView WebView { get; set; }
        public VBox LoginContainer { get; set; }
        public TextEntry ServerIPText { get; set; }
        public Button LogInButton { get; set; }
        public ImageView TrueCraftLogoImage { get; set; }

        public LauncherWindow()
        {
            this.Title = "TrueCraft Launcher";
            this.Width = 1024;
            this.Height = 576;

            MainContainer = new HBox();
            WebScrollView = new ScrollView();
            WebView = new WebView("http://truecraft.io");
            LoginContainer = new VBox();
            ServerIPText = new TextEntry();
            LogInButton = new Button("Log In");
            TrueCraftLogoImage = new ImageView(Image.FromFile("Content/truecraft-logo.png"));

            LoginContainer.MinWidth = 250;
            ServerIPText.PlaceholderText = "Server address";
            LogInButton.Clicked += HandleLogInClicked;

            LoginContainer.PackStart(TrueCraftLogoImage);
            LoginContainer.PackEnd(LogInButton);
            LoginContainer.PackEnd(ServerIPText);
            WebScrollView.Content = WebView;
            MainContainer.PackStart(WebScrollView, true);
            MainContainer.PackEnd(LoginContainer);

            this.Content = MainContainer;
        }

        void HandleLogInClicked(object sender, EventArgs e)
        {
            var process = new Process();
            if (RuntimeInfo.IsMono)
                process.StartInfo = new ProcessStartInfo("mono", "TrueCraft.Client.Linux.exe " + ServerIPText.Text);
            else
                process.StartInfo = new ProcessStartInfo("TrueCraft.Client.Linux.exe", ServerIPText.Text);
            process.EnableRaisingEvents = true;
            process.Exited += (s, a) => Application.Invoke(ClientExited);
            process.Start();
            this.ShowInTaskbar = false;
            this.Hide();
        }

        void ClientExited()
        {
            this.Show();
            this.ShowInTaskbar = true;
        }
    }
}