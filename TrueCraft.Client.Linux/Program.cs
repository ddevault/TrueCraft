using System;

namespace TrueCraft.Client.Linux
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var game = new TrueCraftGame();
            game.Run();
        }
    }
}
