using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TrueCraft.Client.Input;
using TVector3 = TrueCraft.API.Vector3;
using XVector3 = Microsoft.Xna.Framework.Vector3;
using TrueCraft.API;
using TrueCraft.Core.Logic;
using TrueCraft.Core.Networking.Packets;

namespace TrueCraft.Client.Modules
{
    public class PlayerControlModule : IInputModule
    {
        private TrueCraftGame Game { get; set; }
        private DateTime NextAnimation { get; set; }
        private XVector3 Delta { get; set; }
        private bool Capture { get; set; }

        public PlayerControlModule(TrueCraftGame game)
        {
            Game = game;
            Capture = true;
            Game.StartDigging = DateTime.MinValue;
            Game.EndDigging = DateTime.MaxValue;
            Game.TargetBlock = -Coordinates3D.One;
            NextAnimation = DateTime.MaxValue;
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

                case Keys.D1:
                case Keys.NumPad1:
                    Game.Client.HotbarSelection = 0;
                    return true;

                case Keys.D2:
                case Keys.NumPad2:
                    Game.Client.HotbarSelection = 1;
                    return true;

                case Keys.D3:
                case Keys.NumPad3:
                    Game.Client.HotbarSelection = 2;
                    return true;

                case Keys.D4:
                case Keys.NumPad4:
                    Game.Client.HotbarSelection = 3;
                    return true;

                case Keys.D5:
                case Keys.NumPad5:
                    Game.Client.HotbarSelection = 4;
                    return true;

                case Keys.D6:
                case Keys.NumPad6:
                    Game.Client.HotbarSelection = 5;
                    return true;

                case Keys.D7:
                case Keys.NumPad7:
                    Game.Client.HotbarSelection = 6;
                    return true;

                case Keys.D8:
                case Keys.NumPad8:
                    Game.Client.HotbarSelection = 7;
                    return true;

                case Keys.D9:
                case Keys.NumPad9:
                    Game.Client.HotbarSelection = 8;
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
                    if (Game.StartDigging == DateTime.MinValue) // Would like to start digging a block
                    {
                        var target = Game.HighlightedBlock;
                        if (target != -Coordinates3D.One)
                            BeginDigging(target);
                    }
                    else // Currently digging a block
                    {
                        var target = Game.HighlightedBlock;
                        if (target == -Coordinates3D.One) // Cancel
                        {
                            Game.StartDigging = DateTime.MinValue;
                            Game.EndDigging = DateTime.MaxValue;
                            Game.TargetBlock = -Coordinates3D.One;
                        }
                        else if (target != Game.TargetBlock) // Change target
                            BeginDigging(target);
                    }
                    return true;
            }
            return false;
        }

        private void BeginDigging(Coordinates3D target)
        {
            // TODO: Adjust digging time to compensate for latency
            var block = Game.Client.World.GetBlockID(target);
            Game.TargetBlock = target;
            Game.StartDigging = DateTime.UtcNow;
            short damage;
            Game.EndDigging = Game.StartDigging.AddMilliseconds(
                BlockProvider.GetHarvestTime(block, 0, out damage));
            Game.Client.QueuePacket(new PlayerDiggingPacket(
                PlayerDiggingPacket.Action.StartDigging,
                Game.TargetBlock.X, (sbyte)Game.TargetBlock.Y, Game.TargetBlock.Z,
                Game.HighlightedBlockFace));
            NextAnimation = DateTime.UtcNow.AddSeconds(0.25);
        }

        public bool MouseButtonUp(GameTime gameTime, MouseButtonEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButton.Left:
                    Game.StartDigging = DateTime.MinValue;
                    Game.EndDigging = DateTime.MaxValue;
                    Game.TargetBlock = -Coordinates3D.One;
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
            if (Game.EndDigging != DateTime.MaxValue)
            {
                if (NextAnimation < DateTime.UtcNow)
                {
                    NextAnimation = DateTime.UtcNow.AddSeconds(0.25);
                    Game.Client.QueuePacket(new AnimationPacket(Game.Client.EntityID,
                        AnimationPacket.PlayerAnimation.SwingArm));
                }
                if (DateTime.UtcNow > Game.EndDigging && Game.HighlightedBlock == Game.TargetBlock)
                {
                    Game.Client.QueuePacket(new PlayerDiggingPacket(
                        PlayerDiggingPacket.Action.StopDigging,
                        Game.TargetBlock.X, (sbyte)Game.TargetBlock.Y, Game.TargetBlock.Z,
                        Game.HighlightedBlockFace));
                    Game.EndDigging = DateTime.MaxValue;
                }
            }
        }
    }
}
