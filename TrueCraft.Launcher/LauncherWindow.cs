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
        public TextEntry UsernameText { get; set; }
        public PasswordEntry PasswordText { get; set; }
        public TextEntry ServerIPText { get; set; }
        public Button LogInButton { get; set; }
        public Button RegisterButton { get; set; }
        public ImageView TrueCraftLogoImage { get; set; }

        public LauncherWindow()
        {
            this.Title = "TrueCraft Launcher";
            this.Width = 1200;
            this.Height = 576;

            MainContainer = new HBox();
            WebScrollView = new ScrollView();
            WebView = new WebView("http://truecraft.io/updates");
            LoginContainer = new VBox();
            UsernameText = new TextEntry();
            PasswordText = new PasswordEntry();
            ServerIPText = new TextEntry();
            LogInButton = new Button("Log In");
            RegisterButton = new Button("Register");
            TrueCraftLogoImage = new ImageView(Image.FromFile("Content/truecraft-logo.png"));

            LoginContainer.MinWidth = 250;
            ServerIPText.PlaceholderText = "Server address";
            UsernameText.PlaceholderText = "Username";
            PasswordText.PlaceholderText = "Password";
            RegisterButton.Clicked += (sender, e) => WebView.Url = "http://truecraft.io/register";
            LogInButton.Clicked += HandleLogInClicked;

            LoginContainer.PackStart(TrueCraftLogoImage);
            LoginContainer.PackEnd(RegisterButton);
            LoginContainer.PackEnd(LogInButton);
            LoginContainer.PackEnd(ServerIPText);
            LoginContainer.PackEnd(PasswordText);
            LoginContainer.PackEnd(UsernameText);
            WebScrollView.Content = WebView;
            MainContainer.PackStart(WebScrollView, true);
            MainContainer.PackEnd(LoginContainer);

            this.Content = MainContainer;
        }

        void HandleLogInClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ServerIPText.Text) ||
                string.IsNullOrEmpty(UsernameText.Text))
            {
                MessageDialog.ShowError("Username and server IP are required (for now)");
                return;
            }
            var process = new Process();
            if (RuntimeInfo.IsMono)
                process.StartInfo = new ProcessStartInfo("mono", "TrueCraft.Client.exe " + ServerIPText.Text + " " + UsernameText.Text);
            else
                process.StartInfo = new ProcessStartInfo("TrueCraft.Client.exe", ServerIPText.Text + " " + UsernameText.Text);
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
