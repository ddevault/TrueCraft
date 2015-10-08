using System;
using TrueCraft.API.Logic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TrueCraft.Client.Rendering
{
    public static class IconRenderer
    {
        private static Mesh[] BlockMeshes = new Mesh[0x100];
        private static BasicEffect RenderEffect;

        public static void CreateBlocks(TrueCraftGame game, IBlockRepository repository)
        {
            for (int i = 0; i < 0x100; i++)
            {
                var provider = repository.GetBlockProvider((byte)i);
                if (provider == null || provider.GetIconTexture(0) != null)
                    continue;
                int[] indicies;
                var verticies = BlockRenderer.RenderBlock(provider,
                    new BlockDescriptor { ID = provider.ID }, VisibleFaces.All, new Vector3(-0.5f),
                    0, out indicies);
                var mesh = new Mesh(game, verticies, indicies);
                BlockMeshes[provider.ID] = mesh;
            }

            PrepareEffects(game);
        }

        public static void PrepareEffects(TrueCraftGame game)
        {
            RenderEffect = new BasicEffect(game.GraphicsDevice);
            RenderEffect.Texture = game.TextureMapper.GetTexture("terrain.png");
            RenderEffect.TextureEnabled = true;
            RenderEffect.VertexColorEnabled = true;
            RenderEffect.LightingEnabled = true;
            RenderEffect.DirectionalLight0.Direction = new Vector3(10, -10, -0.8f);
            RenderEffect.DirectionalLight0.DiffuseColor = Color.White.ToVector3();
            RenderEffect.DirectionalLight0.Enabled = true;
            RenderEffect.Projection = Matrix.CreateOrthographicOffCenter(
                0, game.GraphicsDevice.Viewport.Width,
                0, game.GraphicsDevice.Viewport.Height,
                0.1f, 1000.0f);
            RenderEffect.View = Matrix.CreateLookAt(Vector3.UnitZ, Vector3.Zero, Vector3.Up);
        }

        public static void RenderItemIcon(SpriteBatch spriteBatch, Texture2D texture, IItemProvider provider,
            byte metadata, Rectangle destination, Color color)
        {
            var icon = provider.GetIconTexture(metadata);
            var scale = texture.Width / 16;
            var source = new Rectangle(icon.Item1 * scale, icon.Item2 * scale, scale, scale);
            spriteBatch.Draw(texture, destination, source, color);
        }

        public static void RenderBlockIcon(TrueCraftGame game, IBlockProvider provider, byte metadata, Rectangle destination)
        {
            var mesh = BlockMeshes[provider.ID];
            if (mesh != null)
            {
                RenderEffect.World = Matrix.Identity
                    * Matrix.CreateScale(0.6f)
                    * Matrix.CreateRotationY(-MathHelper.PiOver4)
                    * Matrix.CreateRotationX(MathHelper.ToRadians(30))
                    * Matrix.CreateScale(new Vector3(destination.Width, destination.Height, 1))
                    * Matrix.CreateTranslation(new Vector3(
                        destination.X, -(destination.Y - game.GraphicsDevice.Viewport.Height / 2) + game.GraphicsDevice.Viewport.Height / 2, 0))
                    * Matrix.CreateTranslation(new Vector3(destination.Width / 2, -destination.Height / 2, 0));
                mesh.Draw(RenderEffect);
            }
        }
    }
}