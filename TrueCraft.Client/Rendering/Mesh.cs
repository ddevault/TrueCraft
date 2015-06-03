using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Linq;

namespace TrueCraft.Client.Rendering
{
    public class Mesh : IDisposable
    {
        public bool Empty { get; set; }
        public VertexBuffer Verticies { get; set; }
        public IndexBuffer Indicies { get; set; }
        public BoundingBox BoundingBox { get; set; }

        private static object GraphicsDeviceLock = new object();

        public Mesh(GraphicsDevice device, VertexPositionNormalTexture[] verticies, int[] indicies, bool calculateBounds = true)
        {
            Empty = verticies.Length == 0 && indicies.Length == 0;
            if (!Empty)
            {
                lock (GraphicsDeviceLock)
                {
                    Verticies = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration,
                        verticies.Length, BufferUsage.WriteOnly);
                    Verticies.SetData(verticies);
                    Indicies = new IndexBuffer(device, typeof(int), indicies.Length, BufferUsage.WriteOnly);
                    Indicies.SetData(indicies);
                }
                if (calculateBounds)
                {
                    BoundingBox = new BoundingBox(
                        verticies.Select(v => v.Position).OrderBy(v => v.Length()).First(),
                        verticies.Select(v => v.Position).OrderByDescending(v => v.Length()).First());
                }
            }
        }

        ~Mesh()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Verticies != null)
                    Verticies.Dispose();
                if (Indicies != null)
                    Indicies.Dispose();
            }

            Verticies = null;
            Indicies = null;
        }

        public void Draw(Effect effect)
        {
            if (Empty)
                return;
            effect.GraphicsDevice.SetVertexBuffer(Verticies);
            effect.GraphicsDevice.Indices = Indicies;
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                    0, 0, Indicies.IndexCount, 0, Indicies.IndexCount / 3);
            }
        }
    }
}