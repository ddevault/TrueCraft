using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TrueCraft.Client.Input
{
    /// <summary>
    /// Encapsulates keyboard input in an event-driven manner.
    /// </summary>
    public sealed class KeyboardComponent : GameComponent
    {
        /// <summary>
        /// Raised when a key for this keyboard component is pressed.
        /// </summary>
        public event EventHandler<KeyboardKeyEventArgs> KeyDown;

        /// <summary>
        /// Raised when a key for this keyboard component is released.
        /// </summary>
        public event EventHandler<KeyboardKeyEventArgs> KeyUp;

        /// <summary>
        /// Gets the state for this keyboard component.
        /// </summary>
        public KeyboardState State { get; private set; }

        /// <summary>
        /// Creates a new keyboard component.
        /// </summary>
        /// <param name="game">The parent game for the component.</param>
        public KeyboardComponent(Game game)
            : base(game)
        {
        }

        /// <summary>
        /// Initializes this keyboard component.
        /// </summary>
        public override void Initialize()
        {
            State = Keyboard.GetState();

            base.Initialize();
        }

        /// <summary>
        /// Updates this keyboard component.
        /// </summary>
        /// <param name="gameTime">The game time for the update.</param>
        public override void Update(GameTime gameTime)
        {
            var newState = Keyboard.GetState();
            Process(newState, State);
            State = newState;

            base.Update(gameTime);
        }

        /// <summary>
        /// Processes a change between two states.
        /// </summary>
        /// <param name="newState">The new state.</param>
        /// <param name="oldState">The old state.</param>
        private void Process(KeyboardState newState, KeyboardState oldState)
        {
            var currentKeys = newState.GetPressedKeys();
            var lastKeys = oldState.GetPressedKeys();

            // LINQ was a saviour here.
            var pressed = currentKeys.Except(lastKeys);
            var unpressed = lastKeys.Except(currentKeys);

            foreach (var key in pressed)
            {
                var args = new KeyboardKeyEventArgs(key, true);
                if (KeyDown != null)
                    KeyDown(this, args);
            }

            foreach (var key in unpressed)
            {
                var args = new KeyboardKeyEventArgs(key, false);
                if (KeyUp != null)
                    KeyUp(this, args);
            }
        }

        /// <summary>
        /// Called when this keyboard component is being disposed of.
        /// </summary>
        /// <param name="disposing">Whether Dispose() called this method.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                KeyDown = null;
                KeyUp = null;
            }

            base.Dispose(disposing);
        }
    }
}
