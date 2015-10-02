using System;
using Microsoft.Xna.Framework.Graphics;
using TrueCraft.API;
using TrueCraft.Client.Rendering;
using Microsoft.Xna.Framework;
using XVector3 = Microsoft.Xna.Framework.Vector3;
using TVector3 = TrueCraft.API.Vector3;
using TRay = TrueCraft.API.Ray;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Client.Modules
{
    public class HighlightModule : IGraphicalModule
    {
        public TrueCraftGame Game { get; set; }

        private BasicEffect HighlightEffect { get; set; }
        private AlphaTestEffect DestructionEffect { get; set; }
        private Mesh ProgressMesh { get; set; }
        private int Progress { get; set; }
        private static readonly RasterizerState RasterizerState;
        private static readonly VertexPositionColor[] CubeVerticies;
        private static readonly short[] CubeIndicies;
        private static readonly BlendState DestructionBlendState;

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
            DestructionBlendState = new BlendState
            {
                ColorSourceBlend = Blend.DestinationColor,
                ColorDestinationBlend = Blend.SourceColor,
                AlphaSourceBlend = Blend.DestinationAlpha,
                AlphaDestinationBlend = Blend.SourceAlpha
            };
            RasterizerState = new RasterizerState
            {
                DepthBias = -3,
                SlopeScaleDepthBias = -3
            };
        }

        public HighlightModule(TrueCraftGame game)
        {
            Game = game;
            HighlightEffect = new BasicEffect(Game.GraphicsDevice);
            HighlightEffect.VertexColorEnabled = true;
            DestructionEffect = new AlphaTestEffect(Game.GraphicsDevice);
            DestructionEffect.Texture = game.TextureMapper.GetTexture("terrain.png");
            DestructionEffect.ReferenceAlpha = 1;

            GenerateProgressMesh();
        }

        private void GenerateProgressMesh()
        {
            int[] indicies;
            var texCoords = new Vector2(Progress, 15);
            var texture = new[]
            {
                texCoords + Vector2.UnitX + Vector2.UnitY,
                texCoords + Vector2.UnitY,
                texCoords,
                texCoords + Vector2.UnitX
            };
            for (int i = 0; i < texture.Length; i++)
                texture[i] *= new Vector2(16f / 256f);
            var verticies = BlockRenderer.CreateUniformCube(XVector3.Zero,
                texture, VisibleFaces.All, 0, out indicies, Color.White);
            ProgressMesh = new Mesh(Game, verticies, indicies);
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
                var provider = Game.BlockRepository.GetBlockProvider(Game.Client.World.GetBlockID(cast.Item1));
                if (provider.InteractiveBoundingBox != null)
                {
                    var box = provider.InteractiveBoundingBox.Value;

                    Game.HighlightedBlock = cast.Item1;
                    Game.HighlightedBlockFace = cast.Item2;

                    DestructionEffect.World = HighlightEffect.World = Matrix.Identity
                        * Matrix.CreateScale(new XVector3((float)box.Width, (float)box.Height, (float)box.Depth))
                        * Matrix.CreateTranslation(new XVector3((float)box.Min.X, (float)box.Min.Y, (float)box.Min.Z))
                        * Matrix.CreateTranslation(new XVector3(cast.Item1.X, cast.Item1.Y, cast.Item1.Z));
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            Game.Camera.ApplyTo(HighlightEffect);
            Game.Camera.ApplyTo(DestructionEffect);

            if (Game.HighlightedBlock != -Coordinates3D.One)
            {
                Game.GraphicsDevice.RasterizerState = RasterizerState;
                foreach (var pass in HighlightEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    HighlightEffect.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                        PrimitiveType.LineList, CubeVerticies, 0,
                        CubeVerticies.Length, CubeIndicies, 0, CubeIndicies.Length / 2);
                }
            }
            if (Game.EndDigging != DateTime.MaxValue)
            {
                var diff = Game.EndDigging - DateTime.UtcNow;
                var total = Game.EndDigging - Game.StartDigging;
                var progress = (int)(diff.TotalMilliseconds / total.TotalMilliseconds * 10);
                progress = -(progress - 5) + 5;
                if (progress > 9)
                    progress = 9;

                if (progress != Progress)
                {
                    Progress = progress;
                    GenerateProgressMesh();
                }

                Game.GraphicsDevice.BlendState = DestructionBlendState;
                ProgressMesh.Draw(DestructionEffect);
                Game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                Game.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            }
        }
    }
}
