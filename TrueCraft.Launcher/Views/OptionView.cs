using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using TrueCraft.Core;
using Xwt;
using Xwt.Drawing;

namespace TrueCraft.Launcher.Views
{
    public class OptionView : VBox
    {
        public LauncherWindow Window { get; set; }
        public Image DefaultImage { get; set; }
        public string DefaultDescription { get; set; }

        public Label OptionLabel { get; set; }
        public Label TexturePackLabel { get; set; }
        public DataField<Image> TexturePackImageField { get; set; }
        public DataField<string> TexturePackNameField { get; set; }
        public DataField<string> TexturePackDescField { get; set; }
        public ListStore TexturePackStore { get; set; }
        public ListView TexturePackListView { get; set; }
        public Button OpenFolderButton { get; set; }
        public Button BackButton { get; set; }

        private List<TexturePack> _texturePacks;
        private TexturePack _lastTexturePack;

        public OptionView(LauncherWindow window)
        {
            DefaultImage = Image.FromFile("Content/default-pack.png");
            DefaultDescription = File.ReadAllText("Content/default-pack.txt");

            _texturePacks = new List<TexturePack>();
            _lastTexturePack = null;

            Window = window;
            this.MinWidth = 250;

            OptionLabel = new Label("Options")
            {
                Font = Font.WithSize(16),
                TextAlignment = Alignment.Center
            };
            TexturePackLabel = new Label("Select a texture pack...");
            TexturePackImageField = new DataField<Image>();
            TexturePackNameField = new DataField<string>();
            TexturePackDescField = new DataField<string>();
            TexturePackStore = new ListStore(TexturePackImageField, TexturePackNameField, TexturePackDescField);
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
            TexturePackListView.Columns.Add("Text", TexturePackNameField, TexturePackDescField);

            TexturePackListView.SelectionChanged += (sender, e) =>
            {
                var texturePack = _texturePacks[TexturePackListView.SelectedRow];
                if (_lastTexturePack != texturePack)
                {
                    UserSettings.Local.SelectedTexturePack = texturePack.Path;
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
                Window.MainContainer.Remove(this);
                Window.MainContainer.PackEnd(Window.MainMenuView);
            };

            LoadTexturePacks();

            this.PackStart(OptionLabel);
            this.PackStart(TexturePackLabel);
            this.PackStart(TexturePackListView);
            this.PackStart(OpenFolderButton);
            this.PackEnd(BackButton);
        }

        private void LoadTexturePacks()
        {
            // We load the default texture pack specially.
            var defaultPack = new TexturePack();
            _texturePacks.Add(defaultPack);
            AddTexturePackRow(defaultPack);

            // Make sure to create the texture pack directory if there is none.
            if (!Directory.Exists(TexturePack.TexturePackPath))
                Directory.CreateDirectory(TexturePack.TexturePackPath);

            var zips = Directory.EnumerateFiles(TexturePack.TexturePackPath);
            foreach (var zip in zips)
            {
                if (!zip.EndsWith(".zip"))
                    continue;

                var texturePack = new TexturePack(zip);
                _texturePacks.Add(texturePack);
                AddTexturePackRow(texturePack);
            }
        }

        private void AddTexturePackRow(TexturePack pack)
        {
            var row = TexturePackStore.AddRow();
            TexturePackStore.SetValue(row, TexturePackImageField, (pack.Image == null) ? DefaultImage.WithSize(IconSize.Medium) : Image.FromStream(pack.Image).WithSize(IconSize.Medium));
            TexturePackStore.SetValue(row, TexturePackNameField, pack.Name);
            TexturePackStore.SetValue(row, TexturePackDescField, pack.Description ?? DefaultDescription);
        }
    }
}
