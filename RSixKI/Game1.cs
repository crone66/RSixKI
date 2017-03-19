using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace RSixKI
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        /*
         * weapon choice (Missing menu)
         * 
         * Doors&Breakables
         * startup arguments (Testing)
         * 
         * 
         * KI (OneNote)
         * KI VS KI
         * Human vs KI
         */

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SceneManager sceneManager;
        private GameScene.GameMode gameMode;
        public static bool Quit = false;
        public static Texture2D Debug;
        public static Texture2D PathDebug;
        public static Game1 CurrentGame;

        public int Iterations = 1;
        public string LevelName;
        public GameScene.GameMode GameMode
        {
            get
            {
                return gameMode;
            }

            set
            {
                gameMode = value;
            }
        }

        public Game1(GameScene.GameMode gameMode, int iterations, string levelName)
        {
            CurrentGame = this;
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 1024;
            
            graphics.ApplyChanges();
            this.gameMode = gameMode;
            LevelName = levelName;
            Iterations = iterations;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            CreateLevelFile();
            CreateWeaponFile();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            WeaponInformation[] weapons = LoadWeaponInfos("Content/Weapons/weapons.xml");
            IsMouseVisible = true;
            Debug = Content.Load<Texture2D>("debug");
            PathDebug = Content.Load<Texture2D>("Textures/hostage");
            //MainMenu (Play, Options, Exit)
            // -> Play
            //      -> Player, Spectator, Training
            //          -> Choose Level (Player)
            //              -> Choose Weapon (Player)
            // -< Options (Resolution, Fullscreen)
            // -> Exit

            sceneManager = new SceneManager();
            GameScene gameScene = new GameScene("GameScene", Services, true, GameMode, "Content/Levels/Training/"+LevelName+".xml", Iterations, spriteBatch);
            gameScene.GameOver += Scene_GameOver;
            gameScene.TrainingFinished += GameScene_TrainingFinished;
            sceneManager.SignIn(gameScene);

            if (gameMode == GameScene.GameMode.PlayAble)
            {
                sceneManager.SignIn(new MenuScene(graphics, "MenuScene", Services, false));
                sceneManager.ChangeScene("MenuScene");
            }
            else
            {
                sceneManager.ChangeScene("GameScene");
            }
        }

        private void GameScene_TrainingFinished(object sender, System.EventArgs e)
        {
            Exit();
        }

        private void Scene_GameOver(object sender, System.EventArgs e)
        {
            if (e == null)
                sceneManager.ChangeScene("MenuScene");
        }

        private static WeaponInformation[] LoadWeaponInfos(string path)
        {
            if (File.Exists(path))
            {
                object result = null;
                XmlSerializer xml = new XmlSerializer(typeof(WeaponContainer));
                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    result = xml.Deserialize(stream);
                }

                if (result != null)
                {
                    WeaponContainer container = (WeaponContainer)result;
                    return container.Weapons;
                }
            }

            return null;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape) || Quit)
                Exit();

            if (GameMode < GameScene.GameMode.Training)
            {
                sceneManager.Update(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

           //if (GameMode <= GameScene.GameMode.Training)
            //{
                sceneManager.Draw(spriteBatch);
            //}

            base.Draw(gameTime);
        }


        private void CreateLevelFile()
        {
            string Name = "level01";

            LevelContainer container = new LevelContainer();
            container.Name = Name;
            container.MapLayout = File.ReadAllLines("Content/Levels/BaseFiles/"+Name+".txt");

            TextureAssignment[] ass = new TextureAssignment[7];
            ass[0] = new TextureAssignment() { Id = 0, Collidable = false, Path = "Content/Textures/0" };
            ass[1] = new TextureAssignment() { Id = 1, Collidable = true, Path = "Content/Textures/1" };
            ass[2] = new TextureAssignment() { Id = 2, Collidable = true, Path = "Content/Textures/2" };
            ass[3] = new TextureAssignment() { Id = 3, Collidable = true, Path = "Content/Textures/3" };
            ass[4] = new TextureAssignment() { Id = 4, Collidable = true, Path = "Content/Textures/4" };
            ass[5] = new TextureAssignment() { Id = 5, Collidable = true, Path = "Content/Textures/5" };
            ass[6] = new TextureAssignment() { Id = 6, Collidable = false, Path = "Content/Textures/6" };
            container.TextureMapping = ass;

            CharacterInformation[] characters = new CharacterInformation[11];
            int id = 0;
            characters[id++] = CreateCharacter(CharacterType.Enemy, 1, 14, 145, new string[2] { "ak12", "p9" });
            characters[id++] = CreateCharacter(CharacterType.Enemy, 4, 11, 180, new string[2] { "mp5", "p9" });
            characters[id++] = CreateCharacter(CharacterType.Enemy, 3, 14, 180, new string[2] { "ak12", "p9" });
            characters[id++] = CreateCharacter(CharacterType.Enemy, 7, 5, 270, new string[2] { "416-c", "p9" });
            characters[id++] = CreateCharacter(CharacterType.Enemy, 7, 9, 325, new string[2] { "g36c", "p9" });
            characters[id++] = CreateCharacter(CharacterType.Enemy, 9, 4, 0, new string[2] { "ak12", "p9" });
            characters[id++] = CreateCharacter(CharacterType.Enemy, 9, 12, 120, new string[2] { "p90", "p9" });
            characters[id++] = CreateCharacter(CharacterType.Enemy, 11, 8, 180, new string[2] { "m1014", "p9" });
            characters[id++] = CreateCharacter(CharacterType.Enemy, 12, 15, 180, new string[2] { "f2", "p9" });
            characters[id++] = CreateCharacter(CharacterType.Player, 17, 10, 270, null);
            characters[id++] = CreateCharacter(CharacterType.Hostage, 12, 6, 270, null);
            
            container.Characters = characters;
            string path = "Content/Levels/Training/"+Name+".xml";

            if (File.Exists(path))
                File.Delete(path);

            XmlSerializer xml = new XmlSerializer(typeof(LevelContainer));
            using (FileStream stream = new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite))
            {
                xml.Serialize(stream, container);
            }
        }

        private void CreateWeaponFile()
        {
            WeaponInformation[] weapons = new WeaponInformation[14];
            weapons[0] = new WeaponInformation("416-c", 30, 4, 740, 3140f, 41, Weapon.AmmoTyp.Clip, Weapon.WeaponSlot.Primary, Equipment.EquipmentType.Useable, "416-c_proj");//mobility = 41, dmg = 42, susp = 35
            weapons[2] = new WeaponInformation("m1014", 8, 8, 500, 1040f, 42, Weapon.AmmoTyp.Shell, Weapon.WeaponSlot.Primary, Equipment.EquipmentType.Useable, "m1014_proj"); //mobility = 42, dmg = 42
            weapons[3] = new WeaponInformation("mp5", 30, 4, 800, 2290, 45, Weapon.AmmoTyp.Clip, Weapon.WeaponSlot.Primary, Equipment.EquipmentType.Useable, "mp5_proj"); //mobility = 45, dmg = 29, susp = 21
            weapons[4] = new WeaponInformation("p90", 50, 3, 970, 2290, 45, Weapon.AmmoTyp.Clip, Weapon.WeaponSlot.Primary, Equipment.EquipmentType.Useable, "p90_proj"); //mobility = 45, dmg = 20, susp = 15
            weapons[5] = new WeaponInformation("sniper", 10, 4, 100, 3120, 20, Weapon.AmmoTyp.Clip, Weapon.WeaponSlot.Primary, Equipment.EquipmentType.Useable, "sniper_proj"); //mobility = 20, dmg = 75
            weapons[6] = new WeaponInformation("ak12", 30, 4, 850, 3130, 40, Weapon.AmmoTyp.Clip, Weapon.WeaponSlot.Primary, Equipment.EquipmentType.Useable, "ak12_proj"); //dmg = 44. 33
            weapons[7] = new WeaponInformation("f2", 30, 4, 980, 3040, 40, Weapon.AmmoTyp.Clip, Weapon.WeaponSlot.Primary, Equipment.EquipmentType.Useable, "f2_proj"); //dmg = 39, 29
            weapons[8] = new WeaponInformation("g36c", 30, 4, 780, 3100, 41, Weapon.AmmoTyp.Clip, Weapon.WeaponSlot.Primary, Equipment.EquipmentType.Useable, "g36c_proj"); //dmg = 38, 28
            weapons[11] = new WeaponInformation("ump45", 25, 4, 600, 2290, 45, Weapon.AmmoTyp.Clip, Weapon.WeaponSlot.Primary, Equipment.EquipmentType.Useable, "ump45_proj"); //mobility = 20, dmg = 38, 28
            weapons[12] = new WeaponInformation("super", 8, 6, 350, 1190, 40, Weapon.AmmoTyp.Shell, Weapon.WeaponSlot.Primary, Equipment.EquipmentType.Useable, "super_proj"); //mobility = 40, dmg = 90, 36
            weapons[13] = new WeaponInformation("m249", 100, 2, 870, 3290, 20, Weapon.AmmoTyp.Clip, Weapon.WeaponSlot.Primary, Equipment.EquipmentType.Useable, "m249_proj"); //mobility = 20, dmg = 33, 24

            weapons[1] = new WeaponInformation("deagle", 7, 8, 330, 3000f, 45, Weapon.AmmoTyp.Clip, Weapon.WeaponSlot.Secondary, Equipment.EquipmentType.Useable, "deagle_proj");//mobility = 45, dmg = 57
            weapons[9] = new WeaponInformation("p9", 16, 5, 360, 2120, 60, Weapon.AmmoTyp.Clip, Weapon.WeaponSlot.Secondary, Equipment.EquipmentType.Useable, "p9_proj"); //mobility = 20, dmg = 30, 25
            weapons[10] = new WeaponInformation("p12", 15, 5, 420, 2200, 60, Weapon.AmmoTyp.Clip, Weapon.WeaponSlot.Secondary, Equipment.EquipmentType.Useable, "p12_proj"); //mobility = 20, dmg = 43, 36

            ProjectileInformation[] projectiles = new ProjectileInformation[14];
            projectiles[0] = new ProjectileInformation("416-c_proj", 42, 35, 0, 3f, 10000f, false);
            projectiles[1] = new ProjectileInformation("deagle_proj", 57, 0, 0, 3f, 10000f, false);
            projectiles[2] = new ProjectileInformation("m1014_proj", 42, 0, 0, 3f, 10000f, false);
            projectiles[3] = new ProjectileInformation("mp5_proj", 29, 21, 0, 3f, 10000f, false);
            projectiles[4] = new ProjectileInformation("p90_proj", 20, 15, 0, 3f, 10000f, false);
            projectiles[5] = new ProjectileInformation("sniper_proj", 75, 0, 0, 3f, 10000f, false);
            projectiles[6] = new ProjectileInformation("ak12_proj", 44, 33, 0, 3f, 10000f, false);
            projectiles[7] = new ProjectileInformation("f2_proj", 39, 29, 0, 3f, 10000f, false);
            projectiles[8] = new ProjectileInformation("g36c_proj", 38, 28, 0, 3f, 10000f, false);
            projectiles[9] = new ProjectileInformation("p9_proj", 30, 25, 0, 3f, 10000f, false);
            projectiles[10] = new ProjectileInformation("p12_proj", 43, 36, 0, 3f, 10000f, false);
            projectiles[11] = new ProjectileInformation("ump45_proj", 38, 28, 0, 3f, 10000f, false);
            projectiles[12] = new ProjectileInformation("super_proj", 90, 46, 0, 3f, 10000f, false);
            projectiles[13] = new ProjectileInformation("m249_proj", 33, 24, 0, 3f, 10000f, false);
            WeaponContainer container = new WeaponContainer(weapons, projectiles);

            string path = "Content/Weapons/weapons.xml";

            if (File.Exists(path))
                File.Delete(path);

            XmlSerializer xml = new XmlSerializer(typeof(WeaponContainer));
            using (FileStream stream = new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite))
            {
                xml.Serialize(stream, container);
            }
        }

        private CharacterInformation CreateCharacter(CharacterType type, int rowIndex, int columnIndex, int rotation, string[] weaponNames, int health = 100)
        {
            CharacterInformation character = new CharacterInformation();
            switch(type)
            {
                case CharacterType.Enemy:
                    character.Name = "Enemy";
                    character.TeamId = 2;
                    break;

                case CharacterType.Player:
                    character.Name = "Player";
                    character.TeamId = 1;
                    break;

                case CharacterType.Hostage:
                    character.Name = "Hostage";
                    character.TeamId = 1;
                    break;
                default:
                    character.Name = "Unknown";
                    character.TeamId = -1;
                    break;
            }
            character.Speed = 0.5f;
            character.RotationSpeed = 0.5f;
            character.RowIndex = rowIndex;
            character.ColumnIndex = columnIndex;
            character.Type = type;
            character.Rotation = rotation;
            character.Health = health;
            character.WeaponNames = weaponNames;
            return character;
        }
    }
}
