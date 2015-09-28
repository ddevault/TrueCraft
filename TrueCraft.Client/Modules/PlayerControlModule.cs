using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TrueCraft.Client.Input;
using TVector3 = TrueCraft.API.Vector3;
using XVector3 = Microsoft.Xna.Framework.Vector3;
using TrueCraft.API;

namespace TrueCraft.Client.Modules
{
    public class PlayerControlModule : IInputModule
    {
        private TrueCraftGame Game { get; set; }
        private DateTime StartDigging { get; set; }
        private DateTime EndDigging { get; set; }
        private Coordinates3D TargetBlock { get; set; }
        private XVector3 Delta { get; set; }
        private bool Capture { get; set; }

        public PlayerControlModule(TrueCraftGame game)
        {
            Game = game;
            Capture = true;
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
                    Delta += XVector3.Left;
                    return true;

                // Move to the right.
                case Keys.D:
                case Keys.Right:
                    Delta += XVector3.Right;
                    return true;

                // Move forwards.
                case Keys.W:
                case Keys.Up:
                    Delta += XVector3.Forward;
                    return true;

                // Move backwards.
                case Keys.S:
                case Keys.Down:
                    Delta += XVector3.Backward;
                    return true;

                case Keys.I:
                    Game.Client.Position = Game.Client.Position.Floor();
                    return true;

                case Keys.Tab:
                    Capture = !Capture;
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
                    Delta -= XVector3.Left;
                    return true;

                // Stop moving to the right.
                case Keys.D:
                case Keys.Right:
                    Delta -= XVector3.Right;
                    return true;

                // Stop moving forwards.
                case Keys.W:
                case Keys.Up:
                    Delta -= XVector3.Forward;
                    return true;

                // Stop moving backwards.
                case Keys.S:
                case Keys.Down:
                    Delta -= XVector3.Backward;
                    return true;
            }
            return false;
        }
        
        public void MouseMove(GameTime gameTime, MouseMoveEventArgs e)
        {
            if (!Capture)
                return;
            var centerX = Game.GraphicsDevice.Viewport.Width / 2;
            var centerY = Game.GraphicsDevice.Viewport.Height / 2;
            Mouse.SetPosition(centerX, centerY);

            var look = new Vector2((centerX - e.X), (centerY - e.Y))
                * (float)(gameTime.ElapsedGameTime.TotalSeconds * 30);

            Game.Client.Yaw -= look.X;
            Game.Client.Pitch -= look.Y;
            Game.Client.Yaw %= 360;
            Game.Client.Pitch = MathHelper.Clamp(Game.Client.Pitch, -89.9f, 89.9f);
        }

        public bool MouseButtonDown(GameTime gameTime, MouseButtonEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButton.Left:
                    return true;
            }
            return false;
        }

        public bool MouseButtonUp(GameTime gameTime, MouseButtonEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButton.Left:
                    return true;
            }
            return false;
        }

        public void Update(GameTime gameTime)
        {
            if (Delta != XVector3.Zero)
            {
                var lookAt = XVector3.Transform(-Delta,
                    Matrix.CreateRotationY(MathHelper.ToRadians(-(Game.Client.Yaw - 180) + 180)));

                lookAt.X *= (float)(gameTime.ElapsedGameTime.TotalSeconds * 4.3717);
                lookAt.Z *= (float)(gameTime.ElapsedGameTime.TotalSeconds * 4.3717);

                Game.Bobbing += Math.Max(Math.Abs(lookAt.X), Math.Abs(lookAt.Z));

                Game.Client.Velocity = new TVector3(lookAt.X, Game.Client.Velocity.Y, lookAt.Z);
            }
            else
                Game.Client.Velocity *= new TVector3(0, 1, 0);
        }
    }
}
