using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;


namespace RSixKI
{
    public class SceneManager
    {
        private Scene currentScene;
        private List<Scene> scenes;

        public Scene CurrentScene
        {
            get
            {
                return currentScene;
            }
        }

        public SceneManager()
        {
            scenes = new List<Scene>();
        }

        public SceneManager(Scene[] scenes)
        {
            this.scenes = new List<Scene>();
            for (int i = 0; i < scenes.Length; i++)
                SignIn(scenes[i]);
        }

        public SceneManager(Scene[] scenes, Scene startScene)
        {
            this.scenes = new List<Scene>();
            for (int i = 0; i < scenes.Length; i++)
                SignIn(scenes[i]);

            ChangeScene(startScene.Name);
        }

        public SceneManager(Scene[] scenes, string startSceneName)
        {
            this.scenes = new List<Scene>();
            for (int i = 0; i < scenes.Length; i++)
                SignIn(scenes[i]);

            ChangeScene(startSceneName);
        }

        public SceneManager(Scene startScene)
        {
            scenes = new List<Scene>();
            SignIn(startScene);
            ChangeScene(startScene.Name);
        }

        public bool ChangeScene(string sceneName)
        {
            if(sceneName != null && sceneName.Length > 0)
            {
                Scene newScene;
                if (SceneExists(sceneName, out newScene))
                {
                    if(currentScene != null)
                        currentScene.OnSceneClosing();
                    currentScene = newScene;
                    currentScene.LoadContent(currentScene.Content);
                    return true;
                }              
            }

            return false;
        }

        public bool SceneExists(string sceneName, out Scene scene)
        {
            scene = null;
            if (sceneName != null && sceneName.Length > 0)
            {
                for (int i = 0; i < scenes.Count; i++)
                {
                    if (scenes[i].Name == sceneName)
                    {
                        scene = scenes[i];
                        return true;
                    }
                }
            }
            return false;
        }

        public bool SceneExists(string sceneName)
        {
            if (sceneName != null && sceneName.Length > 0)
            {
                for (int i = 0; i < scenes.Count; i++)
                {
                    if (scenes[i].Name == sceneName)
                        return true;
                }
            }
            return false;
        }

        public bool SignIn(Scene scene)
        {
            if(scene != null)
            {
                if(!SceneExists(scene.Name))
                {
                    scene.OnSceneChanging += Scene_OnSceneChanging;
                    scenes.Add(scene);
                    return true;
                }
            }

            return false;
        }

        private void Scene_OnSceneChanging(object sender, SceneChangeArgs e)
        {
            if (!ChangeScene(e.NewSceneName))
                Game1.Quit = true; //no open scene (error?)
        }

        public bool SignOut(Scene scene)
        {
            return scenes.Remove(scene);
        }

        public void Update(GameTime gameTime)
        {
            if(CurrentScene != null)
                CurrentScene.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(CurrentScene != null)
                CurrentScene.Draw(spriteBatch);
        }

    }
}
