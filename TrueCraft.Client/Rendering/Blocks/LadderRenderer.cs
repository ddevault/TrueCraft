using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.API.Logic;

namespace TrueCraft.Client.Rendering.Blocks
{
    public class LadderRenderer : BlockRenderer
    {
        static LadderRenderer()
        {
            BlockRenderer.RegisterRenderer(LadderBlock.BlockID, new LadderRenderer());
            for (int i = 0; i < Texture.Length; i++)
                Texture[i] *= new Vector2(16f / 256f);
        }

        private static Vector2 TextureMap = new Vector2(3, 5);
        private static Vector2[] Texture =
        {
            TextureMap + Vector2.UnitX + Vector2.UnitY,
            TextureMap + Vector2.UnitY,
            TextureMap,
            TextureMap + Vector2.UnitX
        };

        public override VertexPositionNormalTexture[] Render(BlockDescriptor descriptor, Vector3 offset,
                                                             Tuple<int, int> textureMap, int indiciesOffset, out int[] indicies)
        {
            VertexPositionNormalTexture[] verticies;
            Vector3 correction;
            int faceCorrection = 0;
            switch ((LadderBlock.LadderDirection)descriptor.Metadata)
            {
                case LadderBlock.LadderDirection.North:
                    verticies = CreateQuad(CubeFace.PositiveZ, offset, Texture, 0, indiciesOffset, out indicies);
                    correction = Vector3.Forward;
                    faceCorrection = (int)CubeFace.PositiveZ * 4;
                    break;
                case LadderBlock.LadderDirection.South:
                    verticies = CreateQuad(CubeFace.NegativeZ, offset, Texture, 0, indiciesOffset, out indicies);
                    correction = Vector3.Backward;
                    faceCorrection = (int)CubeFace.NegativeZ * 4;
                    break;
                case LadderBlock.LadderDirection.East:
                    verticies = CreateQuad(CubeFace.NegativeX, offset, Texture, 0, indiciesOffset, out indicies);
                    correction = Vector3.Right;
                    faceCorrection = (int)CubeFace.NegativeX * 4;
                    break;
                case LadderBlock.LadderDirection.West:
                    verticies = CreateQuad(CubeFace.PositiveX, offset, Texture, 0, indiciesOffset, out indicies);
                    correction = Vector3.Left;
                    faceCorrection = (int)CubeFace.PositiveX * 4;
                    break;
                default:
                    // Should never happen
                    verticies = CreateUniformCube(offset, Texture, indiciesOffset, out indicies);
                    correction = Vector3.Zero;
                    break;
            }
            for (int i = 0; i < verticies.Length; i++)
                verticies[i].Position += correction;
            for (int i = 0; i < indicies.Length; i++)
                indicies[i] -= faceCorrection;
            return verticies;
        }
    }
}