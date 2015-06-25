using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrueCraft.Client.Rendering.Effects
{
    /// <summary>
    /// Represents the ambient lighting state for an effect.
    /// </summary>
    public class AmbientState
    {
        private EffectParameter _color, _intensity;

        /// <summary>
        /// Gets or sets the color for the ambient state.
        /// </summary>
        public Vector3 Color
        {
            get { return _color.GetValueVector3(); }
            set { _color.SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the color for the ambient state.
        /// </summary>
        public float Intensity
        {
            get { return _intensity.GetValueSingle(); }
            set { _intensity.SetValue(value); }
        }

        /// <summary>
        /// Creates a new ambient state.
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="colorName"></param>
        /// <param name="intensityName"></param>
        public AmbientState(Effect effect, string colorName, string intensityName)
        {
            if (effect == null)
                throw new ArgumentException();

            _color = effect.Parameters[colorName];
            _intensity = effect.Parameters[intensityName];
        }
    }
}
