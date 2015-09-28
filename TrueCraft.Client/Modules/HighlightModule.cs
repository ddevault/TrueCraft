using System;
using Microsoft.Xna.Framework.Graphics;
using TrueCraft.API;
using TrueCraft.Client.Rendering;
using Microsoft.Xna.Framework;
using XVector3 = Microsoft.Xna.Framework.Vector3;
using TVector3 = TrueCraft.API.Vector3;
using TRay = TrueCraft.API.Ray;

namespace TrueCraft.Client.Modules
{
    public class HighlightModule : IGraphicalModule
    {
        public TrueCraftGame Game { get; set; }

        private BasicEffect HighlightEffect { get; set; }
        private static readonly VertexPositionColor[] CubeVerticies;
        private static readonly short[] CubeIndicies;

        static HighlightModule()
        {
            var color = Color.Black;
            CubeVerticies = new[]
            {
                new VertexPositionColor(new XVector3(0, 0, 1), color),
                new VertexPositionColor(new XVector3(1, 0, 1), color),
                new VertexPositionColor(new XVector3(1, 1, 1), color),
                new VertexPositionColor(new XVector3(0, 1, 1), color),
                new VertexPositionColor(new XVector3(0, 0, 0), color),
                new VertexPositionColor(new XVector3(1, 0, 0), color),
                new VertexPositionColor(new XVector3(1, 1, 0), color),
                new VertexPositionColor(new XVector3(0, 1, 0), color)
            };
            CubeIndicies = new short[]
            {
                0, 1,   1, 2,   2, 3,   3, 0,
                0, 4,   4, 7,   7, 6,   6, 2,
                1, 5,   5, 4,   3, 7,   6, 5
            };
        }

        public HighlightModule(TrueCraftGame game)
        {
            Game = game;
            HighlightEffect = new BasicEffect(Game.GraphicsDevice);
            HighlightEffect.VertexColorEnabled = true;
        }

        public void Update(GameTime gameTime)
        {
            var direction = XVector3.Transform(XVector3.UnitZ,
                Matrix.CreateRotationX(MathHelper.ToRadians(Game.Client.Pitch)) *
                Matrix.CreateRotationY(MathHelper.ToRadians(-(Game.Client.Yaw - 180) + 180)));

            var cast = VoxelCast.Cast(Game.Client.World,
                new TRay(Game.Camera.Position, new TVector3(direction.X, direction.Y, direction.Z)),
                Game.BlockRepository, TrueCraftGame.Reach, TrueCraftGame.Reach + 2);

            if (cast == null)
                Game.HighlightedBlock = -Coordinates3D.One;
            else
            {
                Game.HighlightedBlock = cast.Item1;
                Game.HighlightedBlockFace = cast.Item2;
                HighlightEffect.World =
                    Matrix.CreateTranslation(new XVector3(-0.5f)) *
                    Matrix.CreateScale(1.01f) *
                    Matrix.CreateTranslation(new XVector3(0.5f)) *
                    Matrix.CreateTranslation(new XVector3(cast.Item1.X, cast.Item1.Y, cast.Item1.Z));
            }
        }

        public void Draw(GameTime gameTime)
        {
            Game.Camera.ApplyTo(HighlightEffect);

            if (Game.HighlightedBlock != -Coordinates3D.One)
            {
                foreach (var pass in HighlightEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    HighlightEffect.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                        PrimitiveType.LineList, CubeVerticies, 0,
                        CubeVerticies.Length, CubeIndicies, 0, CubeIndicies.Length / 2);
                }
            }
        }
    }
}
