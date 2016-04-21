using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace TrueCraft.Client.Modules
{
    public class SkyModule : IGraphicalModule
    {
        // https://github.com/SirCmpwn/TrueCraft/wiki/Sky

        private TrueCraftGame Game { get; set; }
        private BasicEffect SkyPlaneEffect { get; set; }
        private BasicEffect CelestialPlaneEffect { get; set; }
        private VertexBuffer SkyPlane { get; set; }
        private VertexBuffer CelestialPlane { get; set; }

        public SkyModule(TrueCraftGame game)
        {
            Game = game;
            CelestialPlaneEffect = new BasicEffect(Game.GraphicsDevice);
            CelestialPlaneEffect.TextureEnabled = true;

            SkyPlaneEffect = new BasicEffect(Game.GraphicsDevice);
            SkyPlaneEffect.VertexColorEnabled = false;
            SkyPlaneEffect.FogEnabled = true;
            SkyPlaneEffect.FogStart = 0;
            SkyPlaneEffect.FogEnd = 64 * 0.8f;
            SkyPlaneEffect.LightingEnabled = true;
            var plane = new[]
            {
                new VertexPositionColor(new Vector3(-64, 0, -64), Color.White),
                new VertexPositionColor(new Vector3(64, 0, -64), Color.White),
                new VertexPositionColor(new Vector3(-64, 0, 64), Color.White),

                new VertexPositionColor(new Vector3(64, 0, -64), Color.White),
                new VertexPositionColor(new Vector3(64, 0, 64), Color.White),
                new VertexPositionColor(new Vector3(-64, 0, 64), Color.White)
            };
            SkyPlane = new VertexBuffer(Game.GraphicsDevice, VertexPositionColor.VertexDeclaration,
                plane.Length, BufferUsage.WriteOnly);
            SkyPlane.SetData<VertexPositionColor>(plane);
            var celestialPlane = new[]
            {
                new VertexPositionTexture(new Vector3(-60, 0, -60), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(60, 0, -60), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(-60, 0, 60), new Vector2(0, 1)),

                new VertexPositionTexture(new Vector3(60, 0, -60), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(60, 0, 60), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(-60, 0, 60), new Vector2(0, 1))
            };
            CelestialPlane = new VertexBuffer(Game.GraphicsDevice, VertexPositionTexture.VertexDeclaration,
                celestialPlane.Length, BufferUsage.WriteOnly);
            CelestialPlane.SetData<VertexPositionTexture>(celestialPlane);
        }

        private float CelestialAngle
        {
            get
            {
                float x = (Game.Client.World.Time % 24000f) / 24000f - 0.25f;
                if (x < 0) x = 0;
                if (x > 1) x = 1;
                return x + ((1 - ((float)Math.Cos(x * MathHelper.Pi) + 1) / 2) - x) / 3;
            }
        }

        public static Color HSL2RGB(float h, float sl, float l)
        {
            // Thanks http://www.java2s.com/Code/CSharp/2D-Graphics/HSLtoRGBconversion.htm
            float v, r, g, b;
            r = g = b = l;   // default to gray
            v = (l <= 0.5f) ? (l * (1.0f + sl)) : (l + sl - l * sl);
            if (v > 0)
            {
                int sextant;
                float m, sv, fract, vsf, mid1, mid2;
                m = l + l - v;
                sv = (v - m) / v;
                h *= 6.0f;
                sextant = (int)h;
                fract = h - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;
                switch (sextant)
                {
                    case 0:
                        r = v; g = mid1; b = m;
                        break;
                    case 1:
                        r = mid2; g = v; b = m;
                        break;
                    case 2:
                        r = m; g = v; b = mid1;
                        break;
                    case 3:
                        r = m; g = mid2; b = v;
                        break;
                    case 4:
                        r = mid1; g = m; b = v;
                        break;
                    case 5:
                        r = v; g = m; b = mid2;
                        break;
                }
            }
            return new Color(r, g, b);
        }

        private Color BaseColor
        {
            get
            {
                // Note: temperature comes from the current biome, but we have to
                // do biomes differently than Minecraft so we'll un-hardcode this later.
                const float temp = 0.8f / 3;
                return HSL2RGB(0.6222222f - temp * 0.05f, 0.5f + temp * 0.1f, BrightnessModifier);
            }
        }

        public float BrightnessModifier
        {
            get
            {
                var mod = (float)Math.Cos(CelestialAngle * MathHelper.TwoPi) * 2 + 0.5f;
                if (mod < 0) mod = 0;
                if (mod > 1) mod = 1;
                return mod;
            }
        }

        public Color WorldSkyColor
        {
            get
            {
                return BaseColor;
            }
        }

        public Color WorldFogColor
        {
            get
            {
                float y = (float)Math.Cos(CelestialAngle * MathHelper.TwoPi) * 2 + 0.5f;
                return new Color(0.7529412f * y * 0.94f + 0.06f,
                    0.8470588f * y * 0.94f + 0.06f, 1.0f * y * 0.91f + 0.09f);
            }
        }

        public Color AtmosphereColor
        {
            get
            {
                const float blendFactor = 0.29f; // TODO: Compute based on view distance
                Func<float, float, float> blend = (float source, float destination) =>
                    destination + (source - destination) * blendFactor;
                var fog = WorldFogColor.ToVector3();
                var sky = WorldSkyColor.ToVector3();
                var color = new Vector3(blend(sky.X, fog.X), blend(sky.Y, fog.Y), blend(sky.Z, fog.Z));
                // TODO: more stuff
                return new Color(color);
            }
        }

        public void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(AtmosphereColor);
            Game.GraphicsDevice.SetVertexBuffer(SkyPlane);

            var position = Game.Camera.Position;
            var yaw = Game.Camera.Yaw;
            Game.Camera.Position = TrueCraft.API.Vector3.Zero;
            Game.Camera.Yaw = 0;
            Game.Camera.ApplyTo(SkyPlaneEffect);
            Game.Camera.Yaw = yaw;
            Game.Camera.ApplyTo(CelestialPlaneEffect);
            Game.Camera.Position = position;
            // Sky
            SkyPlaneEffect.FogColor = AtmosphereColor.ToVector3();
            SkyPlaneEffect.World = Matrix.CreateRotationX(MathHelper.Pi)
                * Matrix.CreateTranslation(0, 100, 0)
                * Matrix.CreateRotationX(MathHelper.TwoPi * CelestialAngle);
            SkyPlaneEffect.AmbientLightColor = WorldSkyColor.ToVector3(); foreach (var pass in SkyPlaneEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                SkyPlaneEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
            }

            // Sun
            Game.GraphicsDevice.SetVertexBuffer(CelestialPlane);
            var backup = Game.GraphicsDevice.BlendState;
            Game.GraphicsDevice.BlendState = BlendState.Additive;
            Game.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            CelestialPlaneEffect.Texture = Game.TextureMapper.GetTexture("terrain/sun.png");
            CelestialPlaneEffect.World = Matrix.CreateRotationX(MathHelper.Pi)
                * Matrix.CreateTranslation(0, 100, 0)
                * Matrix.CreateRotationX(MathHelper.TwoPi * CelestialAngle);
            foreach (var pass in CelestialPlaneEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                CelestialPlaneEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
            }
            // Moon
            CelestialPlaneEffect.Texture = Game.TextureMapper.GetTexture("terrain/moon.png");
            CelestialPlaneEffect.World = Matrix.CreateTranslation(0, -100, 0)
                * Matrix.CreateRotationX(MathHelper.TwoPi * CelestialAngle);
            foreach (var pass in CelestialPlaneEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                CelestialPlaneEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
            }
            Game.GraphicsDevice.BlendState = backup;
            Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            // Void
            Game.GraphicsDevice.SetVertexBuffer(SkyPlane);
            SkyPlaneEffect.World = Matrix.CreateTranslation(0, -16, 0);
            SkyPlaneEffect.AmbientLightColor = WorldSkyColor.ToVector3()
                * new Vector3(0.2f, 0.2f, 0.6f)
                + new Vector3(0.04f, 0.04f, 0.1f);
            foreach (var pass in SkyPlaneEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                SkyPlaneEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
            }
        }

        public void Update(GameTime gameTime)
        {
        }
    }
}
