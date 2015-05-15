using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TrueCraft.Client.Linux.Rendering
{
    public class BlockRenderer
    {
        private static BlockRenderer DefaultRenderer = new BlockRenderer();

        public static VertexPositionNormalTexture[] RenderBlock(Vector3 offset, Tuple<int, int> textureMap, int indiciesOffset, out int[] indicies)
        {
            return DefaultRenderer.Render(offset, textureMap, indiciesOffset, out indicies);
        }

        public virtual VertexPositionNormalTexture[] Render(Vector3 offset, Tuple<int, int> textureMap, int indiciesOffset, out int[] indicies)
        {
            return CreateUniformCube(offset, textureMap, indiciesOffset, out indicies);
        }

        protected VertexPositionNormalTexture[] CreateUniformCube(Vector3 offset, Tuple<int, int> textureMap, int indiciesOffset, out int[] indicies)
        {
            var texCoords = new Vector2(textureMap.Item1, textureMap.Item2);
            var texture = new[]
            {
                texCoords + Vector2.UnitX + Vector2.UnitY,
                texCoords + Vector2.UnitY,
                texCoords,
                texCoords + Vector2.UnitX
            };
            for (int i = 0; i < texture.Length; i++)
                texture[i] *= new Vector2(16f / 256f);
            indicies = new int[6 * 6];
            var verticies = new VertexPositionNormalTexture[4 * 6];
            int[] _indicies;
            for (int _side = 0; _side < 6; _side++)
            {
                var side = (CubeFace)_side;
                var quad = CreateQuad(side, offset, texture, indiciesOffset, out _indicies);
                Array.Copy(quad, 0, verticies, _side * 4, 4);
                Array.Copy(_indicies, 0, indicies, _side * 6, 6);
            }
            return verticies;
        }

        protected static VertexPositionNormalTexture[] CreateQuad(CubeFace face, Vector3 offset, Vector2[] texture, int indiciesOffset, out int[] indicies)
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

        protected enum CubeFace
        {
            PositiveZ = 0,
            NegativeZ = 1,
            PositiveX = 2,
            NegativeX = 3,
            PositiveY = 4,
            NegativeY = 5
        }

        protected static readonly Vector3[][] CubeMesh;

        protected static readonly Vector3[] CubeNormals =
        {
            new Vector3(0, 0, 1),
            new Vector3(0, 0, -1),
            new Vector3(1, 0, 0),
            new Vector3(-1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, -1, 0)
        };

        static BlockRenderer()
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