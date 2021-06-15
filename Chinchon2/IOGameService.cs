using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Chinchon2
{
    public static class IOGameService
    {
        public static string path = "./game.txt";

        public static void SaveGameState(string path, GameState gameState)
        {
            try
            {
                using StreamWriter fs = File.CreateText(path);
                fs.Write(GameService.SerializeGameState(gameState));
                fs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
