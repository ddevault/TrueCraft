using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TrueCraft.Client.Input;
using TCVector3 = TrueCraft.API.Vector3;

namespace TrueCraft.Client.Modules
{
    public class PlayerControlModule : IInputModule
    {
        private TrueCraftGame Game { get; set; }
        private Vector3 Delta { get; set; }

        public PlayerControlModule(TrueCraftGame game)
        {
            Game = game;
        }

        public bool KeyDown(GameTime gameTime, KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                // Exit game
                case Keys.Escape:
                    Process.GetCurrentProcess().Kill();
                    return true;

                // Take a screenshot.
                case Keys.F2:
                    Game.TakeScreenshot();
                    return true;

                // Move to the left.
                case Keys.A:
                case Keys.Left:
                    Delta += Vector3.Left;
                    return true;

                // Move to the right.
                case Keys.D:
                case Keys.Right:
                    Delta += Vector3.Right;
                    return true;

                // Move forwards.
                case Keys.W:
                case Keys.Up:
                    Delta += Vector3.Forward;
                    return true;

                // Move backwards.
                case Keys.S:
                case Keys.Down:
                    Delta += Vector3.Backward;
                    return true;

                case Keys.Space:
                    if (Math.Floor(Game.Client.Position.Y) == Game.Client.Position.Y)
                        Game.Client.Velocity += TrueCraft.API.Vector3.Up * 0.3;
                    return true;
            }
            return false;
        }

        public bool KeyUp(GameTime gameTime, KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                // Stop moving to the left.
                case Keys.A:
                case Keys.Left:
                    Delta -= Vector3.Left;
                    return true;

                // Stop moving to the right.
                case Keys.D:
                case Keys.Right:
                    Delta -= Vector3.Right;
                    return true;

                // Stop moving forwards.
                case Keys.W:
                case Keys.Up:
                    Delta -= Vector3.Forward;
                    return true;

                // Stop moving backwards.
                case Keys.S:
                case Keys.Down:
                    Delta -= Vector3.Backward;
                    return true;
            }
            return false;
        }
        
        public void MouseMove(GameTime gameTime, MouseMoveEventArgs e)
        {
            var centerX = Game.GraphicsDevice.Viewport.Width / 2;
            var centerY = Game.GraphicsDevice.Viewport.Height / 2;
            Mouse.SetPosition(centerX, centerY);

            var look = new Vector2((centerX - e.X), (centerY - e.Y))
                * (float)(gameTime.ElapsedGameTime.TotalSeconds * 30);

            Game.Client.Yaw += look.X;
            Game.Client.Pitch += look.Y;
            Game.Client.Yaw %= 360;
            Game.Client.Pitch = MathHelper.Clamp(Game.Client.Pitch, -89.9f, 89.9f);
        }

        public void Update(GameTime gameTime)
        {
            if (Delta != Vector3.Zero)
            {
                var lookAt = Vector3.Transform(Delta, Matrix.CreateRotationY(MathHelper.ToRadians(Game.Client.Yaw)));

                lookAt.X *= (float)(gameTime.ElapsedGameTime.TotalSeconds * 4.3717);
                lookAt.Z *= (float)(gameTime.ElapsedGameTime.TotalSeconds * 4.3717);

                Game.Bobbing += Math.Max(Math.Abs(lookAt.X), Math.Abs(lookAt.Z));

                Game.Client.Velocity = new TCVector3(lookAt.X, Game.Client.Velocity.Y, lookAt.Z);
            }
            else
                Game.Client.Velocity *= new TCVector3(0, 1, 0);
        }
    }
}
