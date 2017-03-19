using System;

namespace RSixKI
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            GameScene.GameMode gameMode = GameScene.GameMode.Training;
            int iterations = 5;
            string levelName = "level01";

            CheckStartupArgs(out gameMode, out iterations, out levelName);

            using (var game = new Game1(gameMode, iterations, levelName))
            {
                game.Run();
            }
        }

        static bool CheckStartupArgs(out GameScene.GameMode gameMode, out int iterations, out string levelName)
        {
            gameMode = GameScene.GameMode.Training;
            iterations = 5;
            levelName = "level01";
            if (System.IO.File.Exists("settings.txt"))
            {
                string[] lines = System.IO.File.ReadAllLines("settings.txt");
                if(lines.Length >= 3)
                {
                    if (lines[0] == "Training")
                        gameMode = GameScene.GameMode.Training;
                    else if (lines[0] == "Spectator")
                        gameMode = GameScene.GameMode.Spectator;
                    else if (lines[0] == "Playable")
                        gameMode = GameScene.GameMode.PlayAble;

                    int value = -1;
                    if (int.TryParse(lines[1], out value))
                        iterations = value;

                    levelName = lines[2];
                }

                return true;
            }
            return false;
        }
    }
}
