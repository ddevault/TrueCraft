using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;

namespace TrueCraft.Client.Rendering
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionNormalColorTexture : IVertexType
    {
        public Vector3 Position, Normal;
        public Color Color;
        public Vector2 Texture;

        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new[]
            {
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                new VertexElement(24, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(28, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
            }
        );

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }

        public VertexPositionNormalColorTexture(Vector3 position, Vector3 normal, Color color, Vector2 texture)
        {
            Position = position;
            Normal = normal;
            Color = color;
            Texture = texture;
        }

        public static bool operator ==(VertexPositionNormalColorTexture value1, VertexPositionNormalColorTexture value2)
        {
            return value1.Equals(value2);
        }

        public static bool operator !=(VertexPositionNormalColorTexture value1, VertexPositionNormalColorTexture value2)
        {
            return !value1.Equals(value2);
        }

        public bool Equals(VertexPositionNormalColorTexture other)
        {
            return Position == other.Position && Normal == other.Normal &&
                Color == other.Color && Texture == other.Texture;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            return Equals((VertexPositionNormalColorTexture)obj);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode() ^ Normal.GetHashCode() ^
                Color.GetHashCode() ^ Texture.GetHashCode();
        }
    }
}