using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TrueCraft.Client.Input
{
    /// <summary>
    /// Encapsulates mouse input in an event-driven manner.
    /// </summary>
    public class MouseComponent : GameComponent
    {
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<MouseMoveEventArgs> Move;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<MouseButtonEventArgs> ButtonDown;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<MouseButtonEventArgs> ButtonUp;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<MouseScrollEventArgs> Scroll;

        /// <summary>
        /// Gets the state for this mouse component.
        /// </summary>
        public MouseState State { get; private set; }

        /// <summary>
        /// Creates a new mouse component.
        /// </summary>
        /// <param name="game"></param>
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
        /// <param name="gameTime"></param>
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
        /// <param name="newState"></param>
        /// <param name="last"></param>
        private void Process(MouseState current, MouseState last)
        {
            // Movement.
            if ((current.X != last.X) || (current.Y != last.Y))
            {
                var args = new MouseMoveEventArgs(current.X, current.Y, (current.X - last.X), (current.Y - last.Y));
                if (Move != null)
                    Move(this, args);
            }

            // Scrolling.
            if (current.ScrollWheelValue != last.ScrollWheelValue)
            {
                var args = new MouseScrollEventArgs(current.X, current.Y, current.ScrollWheelValue, (current.ScrollWheelValue - last.ScrollWheelValue));
                if (Scroll != null)
                    Scroll(this, args);
            }

            // A bit of code duplication here, shame XNA doesn't expose button state through an enumeration...

            // Left button.
            if (current.LeftButton != last.LeftButton)
            {
                var args = new MouseButtonEventArgs(current.X, current.Y, MouseButton.Left, (current.LeftButton == ButtonState.Pressed));
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
            if (current.RightButton != last.RightButton)
            {
                var args = new MouseButtonEventArgs(current.X, current.Y, MouseButton.Right, (current.RightButton == ButtonState.Pressed));
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
            if (current.MiddleButton != last.MiddleButton)
            {
                var args = new MouseButtonEventArgs(current.X, current.Y, MouseButton.Middle, (current.MiddleButton == ButtonState.Pressed));
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
        /// Disposes of this mouse component.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            Move = null;
            ButtonDown = null;
            ButtonUp = null;
            Scroll = null;

            base.Dispose(disposing);
        }
    }
}
