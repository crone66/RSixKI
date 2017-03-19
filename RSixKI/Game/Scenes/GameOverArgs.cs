using System;

namespace RSixKI
{ 
    public class GameOverArgs : EventArgs
    {
        public GameScene.GameEndState State;
        public int PlayerHealth;

        public GameOverArgs(GameScene.GameEndState state, int playerHealth)
        {
            State = state;
            PlayerHealth = playerHealth;
        }
    }
}
