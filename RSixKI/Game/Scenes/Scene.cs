using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RSixKI
{
    public abstract class Scene
    {
        public event EventHandler<SceneChangeArgs> OnSceneChanging;
        private string name;
        private ContentManager content;
        protected bool disposeContentOnClose;

        public string Name
        {
            get
            {
                return name;
            }
        }

        public ContentManager Content
        {
            get
            {
                return content;
            }
        }

        public Scene(string name, IServiceProvider service, bool disposeContentOnClose)
        {
            this.name = name;
            this.disposeContentOnClose = disposeContentOnClose;
            content = new ContentManager(service);
        }

        public Scene(string name, ContentManager content, bool disposeContentOnClose)
        {
            this.name = name;
            this.disposeContentOnClose = disposeContentOnClose;
            this.content = content;
        }


        protected virtual void ChangeScene(string newSceneName)
        {
            OnSceneChanging?.Invoke(this, new SceneChangeArgs(newSceneName));
        }

        public virtual void OnSceneClosing()
        {
            if(disposeContentOnClose)
                Content.Unload();
        }

        public abstract void LoadContent(ContentManager content);
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
