using System;
using Microsoft.Xna.Framework.Graphics;
using TrueCraft.API;
using TrueCraft.Client.Rendering;
using Microsoft.Xna.Framework;

namespace TrueCraft.Client.Modules
{
    public class HighlightModule : IGraphicalModule
    {
        public TrueCraftGame Game { get; set; }

        private Texture2D HighlightTexture { get; set; }
        private Coordinates3D HighlightedBlock { get; set; }
        private Mesh HighlightMesh { get; set; }
        private BasicEffect HighlightEffect { get; set; }

        public HighlightModule(TrueCraftGame game)
        {
            Game = game;

            const int size = 64;
            HighlightedBlock = -Coordinates3D.One;
            HighlightTexture = new Texture2D(Game.GraphicsDevice, size, size);

            var colors = new Color[size * size];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = Color.Transparent;
            for (int x = 0; x < size; x++)
                colors[x] = Color.Black; // Top
            for (int x = 0; x < size; x++)
                colors[x + (size - 1) * size] = Color.Black; // Bottom
            for (int y = 0; y < size; y++)
                colors[y * size] = Color.Black; // Left
            for (int y = 0; y < size; y++)
                colors[y * size + (size - 1)] = Color.Black; // Right

            HighlightTexture.SetData<Color>(colors);
            var texcoords = new[]
            {
                Vector2.UnitX + Vector2.UnitY,
                Vector2.UnitY,
                Vector2.Zero,
                Vector2.UnitX
            };
            int[] indicies;
            var verticies = BlockRenderer.CreateUniformCube(Microsoft.Xna.Framework.Vector3.Zero,
                texcoords, VisibleFaces.All, 0, out indicies, Color.White);
            HighlightMesh = new Mesh(Game, verticies, indicies);

            HighlightEffect = new BasicEffect(Game.GraphicsDevice);
            HighlightEffect.EnableDefaultLighting();
            HighlightEffect.DirectionalLight0.SpecularColor = Color.Black.ToVector3();
            HighlightEffect.DirectionalLight1.SpecularColor = Color.Black.ToVector3();
            HighlightEffect.DirectionalLight2.SpecularColor = Color.Black.ToVector3();
            HighlightEffect.TextureEnabled = true;
            HighlightEffect.Texture = HighlightTexture;
            HighlightEffect.VertexColorEnabled = true;
        }

        public void Update(GameTime gameTime)
        {
            var direction = Microsoft.Xna.Framework.Vector3.Transform(
                -Microsoft.Xna.Framework.Vector3.UnitZ,
                Matrix.CreateRotationX(MathHelper.ToRadians(Game.Client.Pitch)) *
                Matrix.CreateRotationY(MathHelper.ToRadians(Game.Client.Yaw)));

            var cast = VoxelCast.Cast(Game.Client.World,
                new TrueCraft.API.Ray(Game.Camera.Position,
                new TrueCraft.API.Vector3(direction.X, direction.Y, direction.Z)),
                Game.BlockRepository, TrueCraftGame.Reach);

            if (cast == null)
                HighlightedBlock = -Coordinates3D.One;
            else
            {
                HighlightedBlock = cast.Item1;
                HighlightEffect.World = Matrix.CreateScale(1.02f) *
                    Matrix.CreateTranslation(new Microsoft.Xna.Framework.Vector3(
                        cast.Item1.X, cast.Item1.Y, cast.Item1.Z));
            }
        }

        public void Draw(GameTime gameTime)
        {
            Game.Camera.ApplyTo(HighlightEffect);

            if (HighlightedBlock != -Coordinates3D.One)
            {
                Game.GraphicsDevice.DepthStencilState = DepthStencilState.None;
                HighlightMesh.Draw(HighlightEffect);
            }
        }
    }
}
