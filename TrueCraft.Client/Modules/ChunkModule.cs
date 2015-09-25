using System;
using TrueCraft.Client.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using TrueCraft.API;

namespace TrueCraft.Client.Modules
{
    public class ChunkModule : IGraphicalModule
    {
        public TrueCraftGame Game { get; set; }
        public ChunkRenderer ChunkRenderer { get; set; }
        public int ChunksRendered { get; set; }

        private List<Mesh> ChunkMeshes { get; set; }
        private ConcurrentBag<Mesh> IncomingChunks { get; set; }

        private BasicEffect OpaqueEffect { get; set; }
        private AlphaTestEffect TransparentEffect { get; set; }

        public ChunkModule(TrueCraftGame game)
        {
            Game = game;

            ChunkRenderer = new ChunkRenderer(Game.Client.World, Game, Game.BlockRepository);
            Game.Client.ChunkLoaded += (sender, e) => ChunkRenderer.Enqueue(e.Chunk);
            //Client.ChunkModified += (sender, e) => ChunkRenderer.Enqueue(e.Chunk, true);
            ChunkRenderer.MeshCompleted += MeshCompleted;
            ChunkRenderer.Start();

            OpaqueEffect = new BasicEffect(Game.GraphicsDevice);
            OpaqueEffect.EnableDefaultLighting();
            OpaqueEffect.DirectionalLight0.SpecularColor = Color.Black.ToVector3();
            OpaqueEffect.DirectionalLight1.SpecularColor = Color.Black.ToVector3();
            OpaqueEffect.DirectionalLight2.SpecularColor = Color.Black.ToVector3();
            OpaqueEffect.TextureEnabled = true;
            OpaqueEffect.Texture = Game.TextureMapper.GetTexture("terrain.png");
            OpaqueEffect.FogEnabled = true;
            OpaqueEffect.FogStart = 512f;
            OpaqueEffect.FogEnd = 1000f;
            OpaqueEffect.FogColor = Color.CornflowerBlue.ToVector3();
            OpaqueEffect.VertexColorEnabled = true;

            TransparentEffect = new AlphaTestEffect(Game.GraphicsDevice);
            TransparentEffect.AlphaFunction = CompareFunction.Greater;
            TransparentEffect.ReferenceAlpha = 127;
            TransparentEffect.Texture = Game.TextureMapper.GetTexture("terrain.png");
            TransparentEffect.VertexColorEnabled = true;

            ChunkMeshes = new List<Mesh>();
            IncomingChunks = new ConcurrentBag<Mesh>();
        }

        void MeshCompleted(object sender, RendererEventArgs<ReadOnlyChunk> e)
        {
            IncomingChunks.Add(e.Result);
        }

        void HandleClientPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Position":
                    var sorter = new ChunkRenderer.ChunkSorter(new Coordinates3D(
                        (int)Game.Client.Position.X, 0, (int)Game.Client.Position.Z));
                    Game.PendingMainThreadActions.Add(() => ChunkMeshes.Sort(sorter));
                    break;
            }
        }

        public void Update(GameTime gameTime)
        {
            Mesh mesh;
            while (IncomingChunks.TryTake(out mesh))
            {
                ChunkMeshes.Add(mesh);
            }
        }

        private static readonly BlendState ColorWriteDisable = new BlendState
        {
            ColorWriteChannels = ColorWriteChannels.None
        };

        public void Draw(GameTime gameTime)
        {
            Game.Camera.ApplyTo(OpaqueEffect);
            Game.Camera.ApplyTo(TransparentEffect);

            int chunks = 0;
            Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            for (int i = 0; i < ChunkMeshes.Count; i++)
            {
                if (Game.Camera.Frustum.Intersects(ChunkMeshes[i].BoundingBox))
                {
                    chunks++;
                    ChunkMeshes[i].Draw(OpaqueEffect, 0);
                }
            }

            Game.GraphicsDevice.BlendState = ColorWriteDisable;
            for (int i = 0; i < ChunkMeshes.Count; i++)
            {
                if (Game.Camera.Frustum.Intersects(ChunkMeshes[i].BoundingBox))
                    ChunkMeshes[i].Draw(TransparentEffect, 1);
            }

            Game.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            for (int i = 0; i < ChunkMeshes.Count; i++)
            {
                if (Game.Camera.Frustum.Intersects(ChunkMeshes[i].BoundingBox))
                    ChunkMeshes[i].Draw(TransparentEffect, 1);
            }

            ChunksRendered = chunks;
        }
    }
}
