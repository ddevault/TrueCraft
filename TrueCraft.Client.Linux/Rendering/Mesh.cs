using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TrueCraft.Client.Linux.Rendering
{
    public class Mesh
    {
        public VertexBuffer Verticies { get; set; }
        public IndexBuffer Indicies { get; set; }

        public Mesh(GraphicsDevice device, VertexPositionNormalTexture[] verticies, int[] indicies)
        {
            Verticies = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration,
                verticies.Length, BufferUsage.WriteOnly);
            Verticies.SetData(verticies);
            Indicies = new IndexBuffer(device, typeof(int), indicies.Length, BufferUsage.WriteOnly);
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

        public static VertexPositionNormalTexture[] CreateQuad(CubeFace face, Vector3 offset, Vector2[] texture, int indiciesOffset, out int[] indicies)
        {
            indicies = new[] { 0, 1, 3, 1, 2, 3 };
            for (int i = 0; i < indicies.Length; i++)
                indicies[i] += ((int)face * 4) + indiciesOffset;
            var quad = new VertexPositionNormalTexture[4];
            var unit = CubeMesh[(int)face];
            var normal = CubeNormals[(int)face];
            for (int i = 0; i < 4; i++)
            {
                quad[i] = new VertexPositionNormalTexture(offset + unit[i], normal, texture[i]);
            }
            return quad;
        }

        public enum CubeFace
        {
            PositiveZ = 0,
            NegativeZ = 1,
            PositiveX = 2,
            NegativeX = 3,
            PositiveY = 4,
            NegativeY = 5
        }

        private static readonly Vector3[][] CubeMesh;

        private static readonly Vector3[] CubeNormals =
        {
            new Vector3(0, 0, 1),
            new Vector3(0, 0, -1),
            new Vector3(1, 0, 0),
            new Vector3(-1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, -1, 0)
        };

        static Mesh()
        {
            CubeMesh = new Vector3[6][];

            CubeMesh[0] = new[] // Positive Z face
            {
                new Vector3(0.5f, -0.5f, 0.5f),
                new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, 0.5f)
            };

            CubeMesh[1] = new[] // Negative Z face
            {
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f)
            };

            CubeMesh[2] = new[] // Positive X face
            {
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, -0.5f)
            };

            CubeMesh[3] = new[] // Negative X face
            {
                new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f)
            };

            CubeMesh[4] = new[] // Positive Y face
            {
                new Vector3(0.5f, 0.5f, 0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(0.5f, 0.5f, -0.5f)
            };

            CubeMesh[5] = new[] // Negative Y face
            {
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, -0.5f, 0.5f)
            };
        }

    }
}