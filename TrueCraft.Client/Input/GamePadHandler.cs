using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace TrueCraft.Client.Input
{
    public class GamePadHandler : GameComponent
    {
        public GamePadState State { get; set; }
        public PlayerIndex PlayerIndex { get; set; }

        public event EventHandler<GamePadButtonEventArgs> ButtonDown;
        public event EventHandler<GamePadButtonEventArgs> ButtonUp;

        public GamePadHandler(Game game) : base(game)
        {
            PlayerIndex = PlayerIndex.One;
        }

        public override void Initialize()
        {
            State = GamePad.GetState(PlayerIndex);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            var newState = GamePad.GetState(PlayerIndex);
            Process(newState, State);
            State = newState;

            base.Update(gameTime);
        }

        private void Process(GamePadState newState, GamePadState oldState)
        {
            if (!newState.IsConnected)
                return;
            if (newState.Buttons != oldState.Buttons)
            {
                var newButtons = Enum.GetValues(typeof(Buttons))
                   .Cast<Buttons>()
                   .Where(newState.IsButtonDown);
                var oldButtons = Enum.GetValues(typeof(Buttons))
                   .Cast<Buttons>()
                   .Where(oldState.IsButtonDown);

                var pressed = newButtons.Except(oldButtons).ToArray();
                var unpressed = oldButtons.Except(newButtons).ToArray();

                foreach (var button in pressed)
                {
                    if (ButtonDown != null)
                        ButtonDown(this, new GamePadButtonEventArgs { Button = button });
                }

                foreach (var button in unpressed)
                {
                    if (ButtonUp != null)
                        ButtonUp(this, new GamePadButtonEventArgs { Button = button });
                }
            }
        }
    }
}