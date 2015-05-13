using System;
using Microsoft.Xna.Framework.Graphics;

namespace TrueCraft.Client.Linux.Rendering
{
    public class Mesh
    {
        public VertexBuffer Verticies { get; set; }
        public IndexBuffer Indicies { get; set; }

        public Mesh(GraphicsDevice device, VertexPositionNormalTexture[] verticies, int[] indicies)
        {
            Verticies = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration,
                verticies.Length, BufferUsage.None);
            Verticies.SetData(verticies);
            Indicies = new IndexBuffer(device, IndexElementSize.ThirtyTwoBits, indicies.Length, BufferUsage.None);
            Indicies.SetData(indicies);
        }

        ~Mesh()
        {
            if (Verticies != null)
                Verticies.Dispose();
            if (Indicies != null)
                Indicies.Dispose();
        }

        public void Draw(Effect effect)
        {
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