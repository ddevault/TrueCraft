using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TrueCraft.Client.Rendering.Effects
{
    /// <summary>
    /// Represents an opaque effect.
    /// </summary>
    public sealed class OpaqueEffect : Effect, IEffectMatrices
    {
        private EffectParameter _world, _view, _projection;
        private EffectParameter _texture;

        /// <summary>
        /// Gets or sets the world matrix for the effect.
        /// </summary>
        public Matrix World
        {
            get { return _world.GetValueMatrix(); }
            set { _world.SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the view matrix for the effect.
        /// </summary>
        public Matrix View
        {
            get { return _view.GetValueMatrix(); }
            set { _view.SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the projection matrix for the effect.
        /// </summary>
        public Matrix Projection
        {
            get { return _projection.GetValueMatrix(); }
            set { _projection.SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the material texture for the effect.
        /// </summary>
        public Texture2D Texture
        {
            get { return _texture.GetValueTexture2D(); }
            set { _texture.SetValue(value); }
        }

        /// <summary>
        /// Gets the ambient lighting state for the effect.
        /// </summary>
        public AmbientState Ambient { get; private set; }

        /// <summary>
        /// Gets the diffuse lighting state for the effect.
        /// </summary>
        public DiffuseState Diffuse { get; private set; }

        /// <summary>
        /// Creates a new opaque effect.
        /// </summary>
        /// <param name="contentManager"></param>
        public OpaqueEffect(ContentManager contentManager)
            : base(contentManager.Load<Effect>("Effects/Opaque.mgfx"))
        {
            _world = Parameters["world"];
            _view = Parameters["view"];
            _projection = Parameters["projection"];
            _texture = Parameters["texture0"];

            Ambient = new AmbientState(this, "ambient", "ambientIntensity");
            Diffuse = new DiffuseState(this, "diffuse", "diffuseIntensity", "diffuseDirection");
        }
    }
}
