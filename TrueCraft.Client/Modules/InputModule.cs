using System;
using TrueCraft.Client.Input;
using Microsoft.Xna.Framework;

namespace TrueCraft.Client.Modules
{
    public abstract class InputModule : IInputModule
    {
        public virtual bool KeyDown(GameTime gameTime, KeyboardKeyEventArgs e)
        {
            return false;
        }

        public virtual bool KeyUp(GameTime gameTime, KeyboardKeyEventArgs e)
        {
            return false;
        }

        public virtual bool MouseMove(GameTime gameTime, MouseMoveEventArgs e)
        {
            return false;
        }

        public virtual bool MouseButtonDown(GameTime gameTime, MouseButtonEventArgs e)
        {
            return false;
        }

        public virtual bool MouseButtonUp(GameTime gameTime, MouseButtonEventArgs e)
        {
            return false;
        }

        public virtual bool MouseScroll(GameTime gameTime, MouseScrollEventArgs e)
        {
            return false;
        }

        public virtual bool GamePadButtonDown(GameTime gameTime, GamePadButtonEventArgs e)
        {
            return false;
        }

        public virtual bool GamePadButtonUp(GameTime gameTime, GamePadButtonEventArgs e)
        {
            return false;
        }

        public virtual void Update(GameTime gameTime)
        {
        }
    }
}