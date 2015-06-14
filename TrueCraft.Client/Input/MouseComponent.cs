using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TrueCraft.Client.Input
{
    /// <summary>
    /// Encapsulates mouse input in an event-driven manner.
    /// </summary>
    public sealed class MouseComponent : GameComponent
    {
        /// <summary>
        /// Raised when this mouse component is moved.
        /// </summary>
        public event EventHandler<MouseMoveEventArgs> Move;

        /// <summary>
        /// Raised when a button for this mouse component is pressed.
        /// </summary>
        public event EventHandler<MouseButtonEventArgs> ButtonDown;

        /// <summary>
        /// Raised when a button for this mouse component is released.
        /// </summary>
        public event EventHandler<MouseButtonEventArgs> ButtonUp;

        /// <summary>
        /// Raised when the scroll wheel for this mouse component is moved.
        /// </summary>
        public event EventHandler<MouseScrollEventArgs> Scroll;

        /// <summary>
        /// Gets the state for this mouse component.
        /// </summary>
        public MouseState State { get; private set; }

        /// <summary>
        /// Creates a new mouse component.
        /// </summary>
        /// <param name="game">The parent game for the component.</param>
        public MouseComponent(Game game)
            : base(game)
        {
        }

        /// <summary>
        /// Initializes this mouse component.
        /// </summary>
        public override void Initialize()
        {
            State = Mouse.GetState();

            base.Initialize();
        }

        /// <summary>
        /// Updates this mouse component.
        /// </summary>
        /// <param name="gameTime">The game time for the update.</param>
        public override void Update(GameTime gameTime)
        {
            var newState = Mouse.GetState();
            Process(newState, State);
            State = newState;

            base.Update(gameTime);
        }

        /// <summary>
        /// Processes a change between two states.
        /// </summary>
        /// <param name="newState">The new state.</param>
        /// <param name="oldState">The old state.</param>
        private void Process(MouseState newState, MouseState oldState)
        {
            // Movement.
            if ((newState.X != oldState.X) || (newState.Y != oldState.Y))
            {
                var args = new MouseMoveEventArgs(newState.X, newState.Y, (newState.X - oldState.X), (newState.Y - oldState.Y));
                if (Move != null)
                    Move(this, args);
            }

            // Scrolling.
            if (newState.ScrollWheelValue != oldState.ScrollWheelValue)
            {
                var args = new MouseScrollEventArgs(newState.X, newState.Y, newState.ScrollWheelValue, (newState.ScrollWheelValue - oldState.ScrollWheelValue));
                if (Scroll != null)
                    Scroll(this, args);
            }

            // A bit of code duplication here, shame XNA doesn't expose button state through an enumeration...

            // Left button.
            if (newState.LeftButton != oldState.LeftButton)
            {
                var args = new MouseButtonEventArgs(newState.X, newState.Y, MouseButton.Left, (newState.LeftButton == ButtonState.Pressed));
                if (args.IsPressed)
                {
                    if (ButtonDown != null)
                        ButtonDown(this, args);
                }
                else
                {
                    if (ButtonUp != null)
                        ButtonUp(this, args);
                }
            }

            // Right button.
            if (newState.RightButton != oldState.RightButton)
            {
                var args = new MouseButtonEventArgs(newState.X, newState.Y, MouseButton.Right, (newState.RightButton == ButtonState.Pressed));
                if (args.IsPressed)
                {
                    if (ButtonDown != null)
                        ButtonDown(this, args);
                }
                else
                {
                    if (ButtonUp != null)
                        ButtonUp(this, args);
                }
            }

            // Middle button.
            if (newState.MiddleButton != oldState.MiddleButton)
            {
                var args = new MouseButtonEventArgs(newState.X, newState.Y, MouseButton.Middle, (newState.MiddleButton == ButtonState.Pressed));
                if (args.IsPressed)
                {
                    if (ButtonDown != null)
                        ButtonDown(this, args);
                }
                else
                {
                    if (ButtonUp != null)
                        ButtonUp(this, args);
                }
            }
        }

        /// <summary>
        /// Called when this mouse component is being disposed of.
        /// </summary>
        /// <param name="disposing">Whether Dispose() called this method.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Move = null;
                ButtonDown = null;
                ButtonUp = null;
                Scroll = null;
            }

            base.Dispose(disposing);
        }
    }
}
