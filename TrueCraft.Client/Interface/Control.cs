using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TrueCraft.Client.Interface
{
    /// <summary>
    /// Abstract base class for uniformly-implemented game interfaces.
    /// </summary>
    public abstract class Control
        : IGameInterface
    {
        private bool _isVisible;

        /// <summary>
        /// Gets or sets whether the control is visible.
        /// </summary>
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible == value)
                    return;

                _isVisible = value;
                if (_isVisible) OnShow();
                else OnHide();
            }
        }

        /// <summary>
        /// Gets or sets the scale for the control.
        /// </summary>
        public InterfaceScale Scale { get; set; }

        /// <summary>
        /// Creates a new control.
        /// </summary>
        protected Control() { Scale = InterfaceScale.Medium; }

        /// <summary>
        /// Shows the control.
        /// </summary>
        public void Show() { IsVisible = true; }

        /// <summary>
        /// Hides the control.
        /// </summary>
        public void Hide() { IsVisible = false; }

        /// <summary>
        /// Called when the control's visibility is set to true.
        /// </summary>
        protected abstract void OnShow();

        /// <summary>
        /// Called when the control is updated.
        /// </summary>
        /// <param name="gameTime"></param>
        protected abstract void OnUpdate(GameTime gameTime);

        /// <summary>
        /// Called when the control is drawn.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        protected abstract void OnDrawSprites(GameTime gameTime, SpriteBatch spriteBatch);

        /// <summary>
        /// Called when the control's visibility is set to false.
        /// </summary>
        protected abstract void OnHide();

        /// <summary>
        /// Updates the control.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            OnUpdate(gameTime);
        }

        /// <summary>
        /// Draws the control.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public virtual void DrawSprites(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (IsVisible)
                OnDrawSprites(gameTime, spriteBatch);
        }

        /// <summary>
        /// Returns the preferred scale factor for the control.
        /// </summary>
        /// <returns></returns>
        protected float GetScaleFactor()
        {
            switch (Scale)
            {
                case InterfaceScale.Small:
                    return 0.5f;

                default:
                case InterfaceScale.Medium:
                    return 1.0f;

                case InterfaceScale.Large:
                    return 1.5f;
            }
        }
    }
}
