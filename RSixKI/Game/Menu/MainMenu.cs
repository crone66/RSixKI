using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace RSixKI
{
    public class MainMenu : Menu
    {
        private Texture2D[] textures;
        private SpriteFont font;

        public MainMenu(Texture2D[] textures, SpriteFont font, string name, GraphicsDeviceManager device, ContentManager content) : base(name, device, content)
        {
            this.textures = textures;
            this.font = font;
        }

        public override void InitControls()
        {
            Button playButton = new Button("play", 0, new Rectangle((width / 2) - 50, (height / 2) - 175, 100, 50), true, false, textures[0], font, Color.Black, Color.Orange, "Play");
            playButton.OnMouseLeftClicked += PlayButton_OnMouseLeftClicked;
            playButton.OnEnter += PlayButton_OnEnter;
            controls.Add(playButton);

            Button settingsButton = new Button("settings", 1, new Rectangle((width / 2) - 50, (height / 2) - 100, 100, 50), true, false, textures[0], font, Color.DarkGray, Color.DarkGray, "Settings");
            //settingsButton.OnMouseLeftClicked += SettingsButton_OnMouseLeftClicked;
            //settingsButton.OnEnter += SettingsButton_OnEnter;
            controls.Add(settingsButton);

            Button exitButton = new Button("exit", 2, new Rectangle((width / 2) - 50, (height / 2) - 25, 100, 50), true, false, textures[0], font, Color.Black, Color.Orange, "Exit");
            exitButton.OnEnter += ExitButton_OnEnter;
            exitButton.OnMouseLeftClicked += ExitButton_OnMouseLeftClicked;
            controls.Add(exitButton);
            base.InitControls();
        }

        public override void ResolutionChanged(int width, int height)
        {
            controls.Clear();
            this.width = width;
            this.height = height;
            InitControls();
        }

        private void PlayButton_OnEnter(object sender, EventArgs e)
        {
            Close("GameScene");
        }

        private void PlayButton_OnMouseLeftClicked(object sender, EventArgs e)
        {
            Close("GameScene");
        }

        private void SettingsButton_OnEnter(object sender, EventArgs e)
        {
            Close("settings");
        }

        private void SettingsButton_OnMouseLeftClicked(object sender, EventArgs e)
        {
            Close("settings");
        }

        private void ExitButton_OnMouseLeftClicked(object sender, EventArgs e)
        {
            Game1.Quit = true;
        }

        private void ExitButton_OnEnter(object sender, EventArgs e)
        {
            Game1.Quit = true;
        }

        public override void LoadContent(ContentManager content)
        {
            
        }
    }
}
