using System;
using Xwt;
using System.Diagnostics;
using Xwt.Drawing;
using TrueCraft.Launcher.Views;
using TrueCraft.Core;
using System.Reflection;

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
        public OptionView OptionView { get; set; }
        public MultiplayerView MultiplayerView { get; set; }
        public SingleplayerView SingleplayerView { get; set; }
        public VBox InteractionBox { get; set; }
        public ImageView TrueCraftLogoImage { get; set; }

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
            OptionView = new OptionView(this);
            MultiplayerView = new MultiplayerView(this);
            SingleplayerView = new SingleplayerView(this);
            InteractionBox = new VBox();

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TrueCraft.Launcher.Content.truecraft_logo.svg"))
                TrueCraftLogoImage = new ImageView(Image.FromStream(stream));

            WebScrollView.Content = WebView;
            MainContainer.PackStart(WebScrollView, true);
            InteractionBox.PackStart(TrueCraftLogoImage);
            InteractionBox.PackEnd(LoginView);
            MainContainer.PackEnd(InteractionBox);

            this.Content = MainContainer;
        }

        void ClientExited()
        {
            this.Show();
            this.ShowInTaskbar = true;
        }
    }
}
