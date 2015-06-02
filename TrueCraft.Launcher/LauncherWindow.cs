using System;
using Xwt;
using System.Diagnostics;
using Xwt.Drawing;
using TrueCraft.Launcher.Views;
using TrueCraft.Core;

namespace TrueCraft.Launcher
{
    public class LauncherWindow : Window
    {
        public TrueCraftUser User { get; set; }

        public HBox MainContainer { get; set; }
        public ScrollView WebScrollView { get; set; }
        public WebView WebView { get; set; }

        public LoginView LoginView { get; set; }
        public MainMenuView MainMenuView { get; set; }
        public MultiplayerView MultiplayerView { get; set; }

        public LauncherWindow()
        {
            this.Title = "TrueCraft Launcher";
            this.Width = 1200;
            this.Height = 576;
            this.User = new TrueCraftUser();

            MainContainer = new HBox();
            WebScrollView = new ScrollView();
            WebView = new WebView("http://truecraft.io/updates");
            LoginView = new LoginView(this);
            MultiplayerView = new MultiplayerView(this);

            WebScrollView.Content = WebView;
            MainContainer.PackStart(WebScrollView, true);
            MainContainer.PackEnd(LoginView);

            this.Content = MainContainer;
        }

        void ClientExited()
        {
            this.Show();
            this.ShowInTaskbar = true;
        }
    }
}
