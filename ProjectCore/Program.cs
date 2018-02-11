using System;
using MonoGameJamProject;

namespace ProjectCore
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new MainClass())
                game.Run();
        }
    }
}
