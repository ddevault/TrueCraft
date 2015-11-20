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
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.API.Logic;

namespace TrueCraft.Client.Modules
{
    public class PlayerControlModule : InputModule
    {
        private TrueCraftGame Game { get; set; }
        private DateTime NextAnimation { get; set; }
        private XVector3 Delta { get; set; }
        private bool Capture { get; set; }
        private bool Digging { get; set; }
        private GamePadState GamePadState { get; set; }

        public PlayerControlModule(TrueCraftGame game)
        {
            Game = game;
            Capture = true;
            Digging = false;
            Game.StartDigging = DateTime.MinValue;
            Game.EndDigging = DateTime.MaxValue;
            Game.TargetBlock = -Coordinates3D.One;
            NextAnimation = DateTime.MaxValue;
            GamePadState = GamePad.GetState(PlayerIndex.One);
        }

        public override bool KeyDown(GameTime gameTime, KeyboardKeyEventArgs e)
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

                case Keys.E:
                    Game.Client.CurrentWindow = Game.Client.Inventory;
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

        public override bool KeyUp(GameTime gameTime, KeyboardKeyEventArgs e)
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

        public override bool GamePadButtonDown(GameTime gameTime, GamePadButtonEventArgs e)
        {
            var selected = Game.Client.HotbarSelection;
            switch (e.Button)
            {
                case Buttons.LeftShoulder:
                    selected--;
                    if (selected < 0)
                        selected = 8;
                    if (selected > 8)
                        selected = 0;
                    Game.Client.HotbarSelection = selected;
                    break;
                case Buttons.RightShoulder:
                    selected++;
                    if (selected < 0)
                        selected = 8;
                    if (selected > 8)
                        selected = 0;
                    Game.Client.HotbarSelection = selected;
                    break;
                case Buttons.A:
                    if (Math.Floor(Game.Client.Position.Y) == Game.Client.Position.Y)
                        Game.Client.Velocity += TrueCraft.API.Vector3.Up * 0.3;
                    break;
            }
            return false;
        }

        public override bool MouseScroll(GameTime gameTime, MouseScrollEventArgs e)
        {
            var selected = Game.Client.HotbarSelection;
            selected += e.DeltaValue > 0 ? -1 : 1;
            if (selected < 0)
                selected = 8;
            if (selected > 8)
                selected = 0;
            Game.Client.HotbarSelection = selected;
            return true;
        }
        
        public override bool MouseMove(GameTime gameTime, MouseMoveEventArgs e)
        {
            if (!Capture)
                return false;
            var centerX = Game.GraphicsDevice.Viewport.Width / 2;
            var centerY = Game.GraphicsDevice.Viewport.Height / 2;
            if (e.X < 10 || e.X > Game.GraphicsDevice.Viewport.Width - 10 ||
                e.Y < 10 || e.Y > Game.GraphicsDevice.Viewport.Height - 10)
            {
                Mouse.SetPosition(centerX, centerY);
            }

            var look = new Vector2((-e.DeltaX), (-e.DeltaY))
                * (float)(gameTime.ElapsedGameTime.TotalSeconds * 30);

            Game.Client.Yaw -= look.X;
            Game.Client.Pitch -= look.Y;
            Game.Client.Yaw %= 360;
            Game.Client.Pitch = MathHelper.Clamp(Game.Client.Pitch, -89.9f, 89.9f);

            return true;
        }

        public override bool MouseButtonDown(GameTime gameTime, MouseButtonEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButton.Left:
                    Digging = true;
                    return true;
                case MouseButton.Right:
                    var item = Game.Client.Inventory.Hotbar[Game.Client.HotbarSelection];
                        Game.Client.QueuePacket(new PlayerBlockPlacementPacket(
                        Game.HighlightedBlock.X, (sbyte)Game.HighlightedBlock.Y, Game.HighlightedBlock.Z,
                        Game.HighlightedBlockFace, item.ID, item.Count, item.Metadata));
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
                BlockProvider.GetHarvestTime(block,
                    Game.Client.Inventory.Hotbar[Game.Client.HotbarSelection].ID, out damage));
            Game.Client.QueuePacket(new PlayerDiggingPacket(
                PlayerDiggingPacket.Action.StartDigging,
                Game.TargetBlock.X, (sbyte)Game.TargetBlock.Y, Game.TargetBlock.Z,
                Game.HighlightedBlockFace));
            NextAnimation = DateTime.UtcNow.AddSeconds(0.25);
        }

        public override bool MouseButtonUp(GameTime gameTime, MouseButtonEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButton.Left:
                    Digging = false;
                    return true;
            }
            return false;
        }

        private void PlayFootstep()
        {
            var coords = (Coordinates3D)Game.Client.BoundingBox.Min.Floor();
            var target = Game.Client.World.GetBlockID(coords);
            if (target == AirBlock.BlockID)
                target = Game.Client.World.GetBlockID(coords + Coordinates3D.Down);
            var provider = Game.BlockRepository.GetBlockProvider(target);
            if (provider.SoundEffect == SoundEffectClass.None)
                return;
            var effect = string.Format("footstep.{0}", Enum.GetName(typeof(SoundEffectClass), provider.SoundEffect).ToLower());
            Game.Audio.PlayPack(effect, 0.5f);
        }

        public override void Update(GameTime gameTime)
        {
            var delta = Delta;

            var gamePad = GamePad.GetState(PlayerIndex.One); // TODO: Can this stuff be done effectively in the GamePadHandler?
            if (gamePad.IsConnected && gamePad.ThumbSticks.Left.Length() != 0)
                delta = new XVector3(gamePad.ThumbSticks.Left.X, 0, gamePad.ThumbSticks.Left.Y);

            var digging = Digging;

            if (gamePad.IsConnected && gamePad.Triggers.Right > 0.5f)
                digging = true;
            if (gamePad.IsConnected && gamePad.Triggers.Left > 0.5f && GamePadState.Triggers.Left < 0.5f)
            {
                var item = Game.Client.Inventory.Hotbar[Game.Client.HotbarSelection];
                Game.Client.QueuePacket(new PlayerBlockPlacementPacket(
                    Game.HighlightedBlock.X, (sbyte)Game.HighlightedBlock.Y, Game.HighlightedBlock.Z,
                    Game.HighlightedBlockFace, item.ID, item.Count, item.Metadata));
            }
            if (gamePad.IsConnected && gamePad.ThumbSticks.Right.Length() != 0)
            {
                var look = -(gamePad.ThumbSticks.Right * 8) * (float)(gameTime.ElapsedGameTime.TotalSeconds * 30);

                Game.Client.Yaw -= look.X;
                Game.Client.Pitch -= look.Y;
                Game.Client.Yaw %= 360;
                Game.Client.Pitch = MathHelper.Clamp(Game.Client.Pitch, -89.9f, 89.9f);
            }

            if (digging)
            {
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
            }
            else
            {
                Game.StartDigging = DateTime.MinValue;
                Game.EndDigging = DateTime.MaxValue;
                Game.TargetBlock = -Coordinates3D.One;
            }

            if (delta != XVector3.Zero)
            {
                var lookAt = XVector3.Transform(-delta,
                                 Matrix.CreateRotationY(MathHelper.ToRadians(-(Game.Client.Yaw - 180) + 180)));

                lookAt.X *= (float)(gameTime.ElapsedGameTime.TotalSeconds * 4.3717);
                lookAt.Z *= (float)(gameTime.ElapsedGameTime.TotalSeconds * 4.3717);

                var bobbing = Game.Bobbing;
                Game.Bobbing += Math.Max(Math.Abs(lookAt.X), Math.Abs(lookAt.Z));

                Game.Client.Velocity = new TVector3(lookAt.X, Game.Client.Velocity.Y, lookAt.Z);

                if ((int)bobbing % 2 == 0 && (int)Game.Bobbing % 2 != 0)
                    PlayFootstep();
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

            GamePadState = gamePad;
        }
    }
}
