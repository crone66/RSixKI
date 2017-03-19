using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace RSixKI
{
    public abstract class Menu
    {
        private string name;
        protected List<Control> controls;
        private int listIndex = -1;
        protected int width;
        protected int height;
        private KeyboardState prevState;


        public string Name
        {
            get
            {
                return name;
            }
        }

        public event EventHandler<CloseArgs> OnClose;

        public Menu(string name, GraphicsDeviceManager device, ContentManager content)
        {
            this.name = name;
            controls = new List<Control>();
            LoadContent(content);
            width = device.PreferredBackBufferWidth;
            height = device.PreferredBackBufferHeight;
        }

        public abstract void LoadContent(ContentManager content);

        public virtual void InitControls()
        {
            controls.Sort();
            for (int i = 0; i < controls.Count; i++)
            {
                if (controls[i].IsSelected)
                {
                    listIndex = i;
                    break;
                }
            }
        }

        public abstract void ResolutionChanged(int width, int height);

        protected void Close(object args)
        {
            OnClose?.Invoke(this, new CloseArgs(args));
        }

        public void Update(GameTime gameTime)
        {
            if (listIndex >= 0)
            {
                KeyboardState state = Keyboard.GetState();
                if ((state.IsKeyDown(Keys.Up) && prevState.IsKeyUp(Keys.Up)) || (state.IsKeyDown(Keys.W) && prevState.IsKeyUp(Keys.W)))
                    GetLower();
                else if ((state.IsKeyDown(Keys.Down) && prevState.IsKeyUp(Keys.Down)) || (state.IsKeyDown(Keys.S) && prevState.IsKeyUp(Keys.S)))
                    GetHigher();

                prevState = state;
            }

            foreach (Control item in controls)
            {
                item.Update(gameTime);
            }
        }

        public void GetHigher()
        {
            int value = controls[listIndex].Index;
            controls[listIndex].Unselect();

            if (listIndex + 1 < controls.Count)
            {
                for (int i = listIndex + 1; i < controls.Count; i++)
                {
                    if (controls[i].Index > value)
                    {
                        controls[i].Select();
                        listIndex = i;
                    }
                }
            }
            else
            {
                controls[0].Select();
                listIndex = 0;
            }
        }

        public void GetLower()
        {
            int value = controls[listIndex].Index;
            controls[listIndex].Unselect();

            if (listIndex - 1 > 0)
            {
                for (int i = listIndex - 1; i > -1; i--)
                {
                    if (controls[i].Index < value)
                    {
                        controls[i].Select();
                        listIndex = i;
                    }
                }
            }
            else
            {
                controls[controls.Count - 1].Select();
                listIndex = controls.Count - 1;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            foreach (Control item in controls)
            {
                item.Draw(spriteBatch);
            }
            spriteBatch.End();
        }
    }
}
