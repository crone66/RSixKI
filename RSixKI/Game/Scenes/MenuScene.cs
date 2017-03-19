using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace RSixKI
{
    public class MenuScene : Scene
    {
        private List<Menu> menus;
        private Menu currentMenu;
        private GraphicsDeviceManager device;
        private bool loaded;

        public MenuScene(GraphicsDeviceManager device, string name, ContentManager content, bool disposeContentOnClose) : base(name, content, disposeContentOnClose)
        {
            this.device = device;
            menus = new List<Menu>();
        }

        public MenuScene(GraphicsDeviceManager device, string name, IServiceProvider service, bool disposeContentOnClose) : base(name, service, disposeContentOnClose)
        {
            this.device = device;
            menus = new List<Menu>();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(currentMenu != null)
                currentMenu.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            if(currentMenu != null)
                currentMenu.Update(gameTime);
        }

        public override void LoadContent(ContentManager content)
        {
            if (disposeContentOnClose || !loaded)
            {
                content.RootDirectory = "Content";

                SpriteFont font = content.Load<SpriteFont>("font");
                Texture2D buttonBackground = content.Load<Texture2D>("button");

                AddMenu(new MainMenu(new Texture2D[1] { buttonBackground }, font, "MainMenu", device, content), true);
                loaded = true;
            }
        }

        private void AddMenu(Menu menu, bool start = false)
        {
            if(menu != null && !MenuExists(menu.Name))
            {
                menu.OnClose += Menu_OnClose;
                menu.InitControls();
                menus.Add(menu);

                if (start)
                    currentMenu = menu;
            }
        }

        private bool MenuExists(string menuName)
        {
            for (int i = 0; i < menus.Count; i++)
            {
                if (menus[i].Name == menuName)
                    return true;
            }
            return false;
        }

        private void Menu_OnClose(object sender, CloseArgs e)
        {
            if (e.Args != null && menus != null)
            {
                for (int i = 0; i < menus.Count; i++)
                {
                    if (menus[i].Name == e.Args.ToString())
                    {
                        currentMenu = menus[i];
                        return;
                    }
                }

                ChangeScene(e.Args.ToString());
            }
        }
    }
}
