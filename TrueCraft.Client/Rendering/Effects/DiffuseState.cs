using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrueCraft.Client.Rendering.Effects
{
    /// <summary>
    /// Represents the diffuse lighting state for an effect.
    /// </summary>
    public class DiffuseState
    {
        private EffectParameter _color, _intensity, _direction;

        /// <summary>
        /// Gets or sets the color for the diffuse state.
        /// </summary>
        public Vector3 Color
        {
            get { return _color.GetValueVector3(); }
            set { _color.SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the color for the diffuse state.
        /// </summary>
        public float Intensity
        {
            get { return _intensity.GetValueSingle(); }
            set { _intensity.SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the direction for the diffuse state.
        /// </summary>
        public Vector3 Direction
        {
            get { return _direction.GetValueVector3(); }
            set { _direction.SetValue(value); }
        }

        /// <summary>
        /// Creates a new diffuse state.
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="colorName"></param>
        /// <param name="intensityName"></param>
        /// <param name="directionName"></param>
        public DiffuseState(Effect effect, string colorName, string intensityName, string directionName)
        {
            if (effect == null)
                throw new ArgumentException();

            _color = effect.Parameters[colorName];
            _intensity = effect.Parameters[intensityName];
            _direction = effect.Parameters[directionName];
        }
    }
}
