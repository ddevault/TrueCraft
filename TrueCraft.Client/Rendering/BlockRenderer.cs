using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TrueCraft.Core.Logic;
using TrueCraft.API.Logic;
using System.Linq;

namespace TrueCraft.Client.Rendering
{
    public class BlockRenderer
    {
        private static BlockRenderer DefaultRenderer = new BlockRenderer();
        private static BlockRenderer[] Renderers = new BlockRenderer[0x100];

        public static void RegisterRenderer(byte id, BlockRenderer renderer)
        {
            Renderers[id] = renderer;
        }

        public static VertexPositionNormalColorTexture[] RenderBlock(IBlockProvider provider, BlockDescriptor descriptor,
            VisibleFaces faces, Vector3 offset, int indiciesOffset, out int[] indicies)
        {
            var textureMap = provider.GetTextureMap(descriptor.Metadata);
            if (textureMap == null)
                textureMap = new Tuple<int, int>(0, 0); // TODO: handle this better
            return Renderers[descriptor.ID].Render(descriptor, offset, faces, textureMap, indiciesOffset, out indicies);
        }

        public virtual VertexPositionNormalColorTexture[] Render(BlockDescriptor descriptor, Vector3 offset,
            VisibleFaces faces, Tuple<int, int> textureMap, int indiciesOffset, out int[] indicies)
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
            return CreateUniformCube(offset, texture, faces, indiciesOffset, out indicies, Color.White, descriptor.BlockLight);
        }

        public static readonly Random Random = new Random();

        public static VertexPositionNormalColorTexture[] CreateUniformCube(Vector3 offset, Vector2[] texture,
            VisibleFaces faces, int indiciesOffset, out int[] indicies, Color color, int light = 15)
        {
            faces = VisibleFaces.All; // Temporary

            int totalFaces = 0;
            uint f = (uint)faces;
            while (f != 0)
            {
                if ((f & 1) == 1)
                    totalFaces++;
                f >>= 1;
            }

            indicies = new int[6 * totalFaces];
            var verticies = new VertexPositionNormalColorTexture[4 * totalFaces];
            int[] _indicies;
            int textureIndex = 0;
            int sidesSoFar = 0;
            for (int _side = 0; _side < 6; _side++)
            {
                if ((faces & VisibleForCubeFace[_side]) == 0)
                {
                    textureIndex += 4;
                    continue;
                }
                var side = (CubeFace)_side;
                var quad = CreateQuad(side, offset, texture, textureIndex % texture.Length, indiciesOffset,
                    out _indicies, new Color(color.ToVector3() * CubeBrightness[light]));
                Array.Copy(quad, 0, verticies, sidesSoFar * 4, 4);
                Array.Copy(_indicies, 0, indicies, sidesSoFar * 6, 6);
                textureIndex += 4;
                sidesSoFar++;
            }
            return verticies;
        }

        protected static VertexPositionNormalColorTexture[] CreateQuad(CubeFace face, Vector3 offset,
            Vector2[] texture, int textureOffset, int indiciesOffset, out int[] indicies, Color color)
        {
            indicies = new[] { 0, 1, 3, 1, 2, 3 };
            for (int i = 0; i < indicies.Length; i++)
                indicies[i] += ((int)face * 4) + indiciesOffset;
            var quad = new VertexPositionNormalColorTexture[4];
            var unit = CubeMesh[(int)face];
            var normal = CubeNormals[(int)face];
            var faceColor = new Color(FaceBrightness[(int)face] * color.ToVector3());
            for (int i = 0; i < 4; i++)
            {
                quad[i] = new VertexPositionNormalColorTexture(offset + unit[i], normal, faceColor, texture[textureOffset + i]);
            }
            return quad;
        }
        
        protected static readonly float[] FaceBrightness =
            new float[]
            {
                0.6f, 0.6f, // North / South
                0.8f, 0.8f, // East / West
                1.0f, 0.5f  // Top / Bottom
            };

        protected static readonly float[] CubeBrightness =
            new float[]
            {
                0.050f, 0.067f, 0.085f, 0.106f, // [ 0..3 ]
                0.129f, 0.156f, 0.186f, 0.221f, // [ 4..7 ]
                0.261f, 0.309f, 0.367f, 0.437f, // [ 8..11]
                0.525f, 0.638f, 0.789f, 1.000f //  [12..15]
            };

        protected enum CubeFace
        {
            PositiveZ = 0,
            NegativeZ = 1,
            PositiveX = 2,
            NegativeX = 3,
            PositiveY = 4,
            NegativeY = 5
        }

        protected static readonly VisibleFaces[] VisibleForCubeFace =
        {
            VisibleFaces.South,
            VisibleFaces.North,
            VisibleFaces.East,
            VisibleFaces.West,
            VisibleFaces.Top,
            VisibleFaces.Bottom
        };

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
            for (int i = 0; i < Renderers.Length; i++)
            {
                Renderers[i] = DefaultRenderer;
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes().Where(t =>
                    typeof(BlockRenderer).IsAssignableFrom(t) && !t.IsAbstract && t != typeof(BlockRenderer)))
                {
                    Activator.CreateInstance(type); // This is just to call the static initializers
                }
            }

            CubeMesh = new Vector3[6][];

            CubeMesh[0] = new[] // Positive Z face
            {
                new Vector3(1, 0, 1),
                new Vector3(0, 0, 1),
                new Vector3(0, 1, 1),
                new Vector3(1, 1, 1)
            };

            CubeMesh[1] = new[] // Negative Z face
            {
                new Vector3(0, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(1, 1, 0),
                new Vector3(0, 1, 0)
            };

            CubeMesh[2] = new[] // Positive X face
            {
                new Vector3(1, 0, 0),
                new Vector3(1, 0, 1),
                new Vector3(1, 1, 1),
                new Vector3(1, 1, 0)
            };

            CubeMesh[3] = new[] // Negative X face
            {
                new Vector3(0, 0, 1),
                new Vector3(0, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, 1, 1)
            };

            CubeMesh[4] = new[] // Positive Y face
            {
                new Vector3(1, 1, 1),
                new Vector3(0, 1, 1),
                new Vector3(0, 1, 0),
                new Vector3(1, 1, 0)
            };

            CubeMesh[5] = new[] // Negative Y face
            {
                new Vector3(1, 0, 0),
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 1),
                new Vector3(1, 0, 1)
            };
        }
    }
}
