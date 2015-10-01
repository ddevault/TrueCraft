using System;
using Microsoft.Xna.Framework.Input;

namespace TrueCraft.Client.Input
{
    public class GamePadButtonEventArgs : GamePadEventArgs
    {
        public Buttons Button { get; set; }
    }
}