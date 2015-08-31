using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using TrueCraft.Core;
using Xwt;
using Xwt.Drawing;
using System.Threading.Tasks;
using System.Net;
using Ionic.Zip;
using System.Linq;

namespace TrueCraft.Launcher.Views
{
    public class OptionView : VBox
    {
        public LauncherWindow Window { get; set; }

        public Label OptionLabel { get; set; }
        public Label ResolutionLabel { get; set; }
        public ComboBox ResolutionComboBox { get; set; }
        public CheckBox FullscreenCheckBox { get; set; }
        public Label TexturePackLabel { get; set; }
        public DataField<Image> TexturePackImageField { get; set; }
        public DataField<string> TexturePackTextField { get; set; }
        public ListStore TexturePackStore { get; set; }
        public ListView TexturePackListView { get; set; }
        public Button OfficialAssetsButton { get; set; }
        public ProgressBar OfficialAssetsProgress { get; set; }
        public Button OpenFolderButton { get; set; }
        public Button BackButton { get; set; }

        private List<TexturePack> _texturePacks;
        private TexturePack _lastTexturePack;

        public OptionView(LauncherWindow window)
        {
            _texturePacks = new List<TexturePack>();
            _lastTexturePack = null;

            Window = window;
            this.MinWidth = 250;

            OptionLabel = new Label("Options")
            {
                Font = Font.WithSize(16),
                TextAlignment = Alignment.Center
            };

            ResolutionLabel = new Label("Select a resolution...");
            ResolutionComboBox = new ComboBox();

            int resolutionIndex = -1;
            for (int i = 0; i < WindowResolution.Defaults.Length; i++)
            {
                ResolutionComboBox.Items.Add(WindowResolution.Defaults[i].ToString());

                if (resolutionIndex == -1)
                {
                    resolutionIndex =
                        ((WindowResolution.Defaults[i].Width == UserSettings.Local.WindowResolution.Width) &&
                        (WindowResolution.Defaults[i].Height == UserSettings.Local.WindowResolution.Height)) ? i : -1;
                }
            }

            if (resolutionIndex == -1)
            {
                ResolutionComboBox.Items.Add(UserSettings.Local.WindowResolution.ToString());
                resolutionIndex = ResolutionComboBox.Items.Count - 1;
            }

            ResolutionComboBox.SelectedIndex = resolutionIndex;
            FullscreenCheckBox = new CheckBox()
            {
                Label = "Fullscreen mode",
                State = (UserSettings.Local.IsFullscreen) ? CheckBoxState.On : CheckBoxState.Off
            };

            TexturePackLabel = new Label("Select a texture pack...");
            TexturePackImageField = new DataField<Image>();
            TexturePackTextField = new DataField<string>();
            TexturePackStore = new ListStore(TexturePackImageField, TexturePackTextField);
            TexturePackListView = new ListView
            {
                MinHeight = 200,
                SelectionMode = SelectionMode.Single,
                DataSource = TexturePackStore,
                HeadersVisible = false
            };
            OpenFolderButton = new Button("Open texture pack folder");
            BackButton = new Button("Back");

            TexturePackListView.Columns.Add("Image", TexturePackImageField);
            TexturePackListView.Columns.Add("Text", TexturePackTextField);

            ResolutionComboBox.SelectionChanged += (sender, e) =>
            {
                UserSettings.Local.WindowResolution =
                    WindowResolution.FromString(ResolutionComboBox.SelectedText);
                UserSettings.Local.Save();
            };

            FullscreenCheckBox.Clicked += (sender, e) =>
            {
                UserSettings.Local.IsFullscreen = !UserSettings.Local.IsFullscreen;
                UserSettings.Local.Save();
            };

            TexturePackListView.SelectionChanged += (sender, e) =>
            {
                var texturePack = _texturePacks[TexturePackListView.SelectedRow];
                if (_lastTexturePack != texturePack)
                {
                    UserSettings.Local.SelectedTexturePack = texturePack.Name;
                    UserSettings.Local.Save();
                }
            };

            OpenFolderButton.Clicked += (sender, e) =>
            {
                var dir = new DirectoryInfo(TexturePack.TexturePackPath);
                Process.Start(dir.FullName);
            };

            BackButton.Clicked += (sender, e) =>
            {
                Window.InteractionBox.Remove(this);
                Window.InteractionBox.PackEnd(Window.MainMenuView);
            };

            OfficialAssetsButton = new Button("Download Minecraft assets") { Visible = false };
            OfficialAssetsButton.Clicked += OfficialAssetsButton_Clicked;
            OfficialAssetsProgress = new ProgressBar() { Visible = false, Indeterminate = true };

            LoadTexturePacks();

            this.PackStart(OptionLabel);
            this.PackStart(ResolutionLabel);
            this.PackStart(ResolutionComboBox);
            this.PackStart(FullscreenCheckBox);
            this.PackStart(TexturePackLabel);
            this.PackStart(TexturePackListView);
            this.PackStart(OfficialAssetsProgress);
            this.PackStart(OfficialAssetsButton);
            this.PackStart(OpenFolderButton);
            this.PackEnd(BackButton);
        }

        void OfficialAssetsButton_Clicked(object sender, EventArgs e)
        {
            var result = MessageDialog.AskQuestion("Download Mojang assets",
                "This will download the official Minecraft assets from Mojang.\n\n" +
                "By proceeding you agree to the Mojang asset guidelines:\n\n" +
                "https://account.mojang.com/terms#brand\n\n" +
                "Proceed?",
                Command.Yes, Command.No);
            if (result == Command.Yes)
            {
                OfficialAssetsButton.Visible = false;
                OfficialAssetsProgress.Visible = true;
                Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            var stream = new WebClient().OpenRead("http://s3.amazonaws.com/Minecraft.Download/versions/b1.7.3/b1.7.3.jar");
                            var ms = new MemoryStream();
                            CopyStream(stream, ms);
                            ms.Seek(0, SeekOrigin.Begin);
                            stream.Dispose();
                            var jar = ZipFile.Read(ms);
                            var zip = new ZipFile();
                            zip.AddEntry("pack.txt", "Minecraft textures");
                            CopyBetweenZips("pack.png", jar, zip);
                            CopyBetweenZips("terrain.png", jar, zip);
                            // TODO: Items, windows, etc
                            zip.Save(Path.Combine(TexturePack.TexturePackPath, "Minecraft.zip"));
                            Application.Invoke(() =>
                                {
                                    OfficialAssetsProgress.Visible = false;
                                    var texturePack = TexturePack.FromArchive(Path.Combine(TexturePack.TexturePackPath, "Minecraft.zip"));
                                    _texturePacks.Add(texturePack);
                                    AddTexturePackRow(texturePack);
                                });
                            ms.Dispose();
                        }
                        catch (Exception ex)
                        {
                            Application.Invoke(() =>
                                {
                                    MessageDialog.ShowError("Error retrieving assets", ex.ToString());
                                    OfficialAssetsProgress.Visible = false;
                                    OfficialAssetsButton.Visible = true;
                                });
                        }
                    });
            }
        }

        public static void CopyBetweenZips(string name, ZipFile source, ZipFile destination)
        {
            using (var stream = source.Entries.SingleOrDefault(f => f.FileName == name).OpenReader())
            {
                var ms = new MemoryStream();
                CopyStream(stream, ms);
                ms.Seek(0, SeekOrigin.Begin);
                destination.AddEntry(name, ms);
            }
        }

        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[16*1024];
            int read;
            while((read = input.Read (buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }

        private void LoadTexturePacks()
        {
            // We load the default texture pack specially.
            _texturePacks.Add(TexturePack.Default);
            AddTexturePackRow(TexturePack.Default);

            // Make sure to create the texture pack directory if there is none.
            if (!Directory.Exists(TexturePack.TexturePackPath))
                Directory.CreateDirectory(TexturePack.TexturePackPath);

            var zips = Directory.EnumerateFiles(TexturePack.TexturePackPath);
            bool officialPresent = false;
            foreach (var zip in zips)
            {
                if (!zip.EndsWith(".zip"))
                    continue;
                if (Path.GetFileName(zip) == "Minecraft.zip")
                    officialPresent = true;

                var texturePack = TexturePack.FromArchive(zip);
                if (texturePack != null)
                {
                    _texturePacks.Add(texturePack);
                    AddTexturePackRow(texturePack);
                }
            }
            if (!officialPresent)
                OfficialAssetsButton.Visible = true;
        }

        private void AddTexturePackRow(TexturePack pack)
        {
            var row = TexturePackStore.AddRow();

            TexturePackStore.SetValue(row, TexturePackImageField, Image.FromStream(pack.Image).WithSize(IconSize.Medium));
            TexturePackStore.SetValue(row, TexturePackTextField, pack.Name + "\r\n" + pack.Description);
        }
    }
}
