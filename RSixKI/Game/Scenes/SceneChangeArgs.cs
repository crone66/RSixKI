using System;

namespace RSixKI
{
    public class SceneChangeArgs : EventArgs
    {
        public string NewSceneName;
        public SceneChangeArgs(string newSceneName)
        {
            NewSceneName = newSceneName;
        }
    }
}
