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
                    // TODO: Enforce a file structure so only certain named files are extracted.
                    texturePack.ExtractTo("Content");
                    _lastTexturePack = texturePack;
                }
            };

            OpenFolderButton.Clicked += (sender, e) =>
            {
                // TODO: Implement cross-platform logic here.
                var dir = new DirectoryInfo("./Content/TexturePacks");
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
            var zips = Directory.EnumerateFiles("Content/TexturePacks/");
            foreach (var zip in zips)
            {
                var texturePack = new TexturePack(zip);
                _texturePacks.Add(texturePack);

                var row = TexturePackStore.AddRow();
                TexturePackStore.SetValue(row, TexturePackImageField, Image.FromStream(texturePack.Image).WithSize(IconSize.Medium));
                TexturePackStore.SetValue(row, TexturePackNameField, texturePack.Name);
                TexturePackStore.SetValue(row, TexturePackDescField, texturePack.Description);
            }
        }
    }
}
