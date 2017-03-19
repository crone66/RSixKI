using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework.Content;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace RSixKI
{
    public class GameScene : Scene
    {
        public enum GameMode
        {
            PlayAble = 0,
            Training = 1,
            Spectator = 2,
        }

        public enum GameEndState
        {
            None,
            PlayerDied,
            HostageDied,
            AllEnemiesKilled,
            HostageRescued,
            TimeExpired,
            NoAction,
        }

        private Level level;
        private EntityManager entityManager;
        private GameMode gameMode;
        private int iterations;
        private Player player;
        private Thread thread;
        private bool playerIsDead;
        private bool enmiesAreDead;
        private string levelPath;
        private SpriteBatch spriteBatch;
        private string primaryWeaponName, secondaryWeaponName;
        private bool primarySuspressed, secondarySuspressed;
        private List<NPC> enemies;
        private Dictionary<string, Texture2D> loadedTextures;
        private List<Vector2> waypoints;
        private Vector2[] entryPositions;
        private Genetics.BalancingGeneticAlgorithm balacing;
        private Genetics.Individuum<Genetics.Decision> best;
        private Stopwatch sw;

        public Dictionary<string, Texture2D> LoadedTextures
        {
            get
            {
                return loadedTextures;
            }
        }

        public event EventHandler<GameOverArgs> GameOver;
        public event EventHandler TrainingFinished;

        public GameScene(string name, IServiceProvider service, bool disposeContentOnClose, GameMode gameMode, string levelPath, int iterations = 1, SpriteBatch spriteBatch = null, string primaryWeaponName = null, string secondaryWeaponName = null, bool primarySuspressed = false, bool secondarySuspressed = false) : base(name, service, disposeContentOnClose)
        {
            this.gameMode = gameMode;
            this.iterations = iterations;
            this.levelPath = levelPath;
            this.spriteBatch = spriteBatch;
            this.primaryWeaponName = primaryWeaponName;
            this.secondaryWeaponName = secondaryWeaponName;
            this.primarySuspressed = primarySuspressed;
            this.secondarySuspressed = secondarySuspressed;
        }

        private void DmgNPC_OnDeath(object sender, HealthChangedArgs e)
        {
            DamageAbleObject dmgObj = sender as DamageAbleObject;
            if(dmgObj != null && dmgObj.Parent != null && dmgObj.Parent is NPC)
            {
                enemies.Remove(dmgObj.Parent as NPC);
            }
            
            if (enemies.Count == 0)
                enmiesAreDead = true;
        }

        /// <summary>
        /// Starts Training
        /// </summary>
        private void StartTraining()
        {
            balacing.IterationCompleted += Balacing_IterationCompleted;
            balacing.IterationStarted += Balacing_IterationStarted;
            Genetics.Individuum<Genetics.Decision> best = balacing.Do();
            Console.WriteLine("Best: " + best.Fitness.ToString()+ ", Iterations: " + iterations.ToString() + " => results saved!");
            TrainingFinished?.Invoke(this, EventArgs.Empty);
        }

        private void Balacing_IterationStarted(object sender, EventArgs e)
        {
            sw = new Stopwatch();
            sw.Start();
        }

        /// <summary>
        /// Outputs best individuum after each iteration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Balacing_IterationCompleted(object sender, Genetics.IterationArgs<Genetics.Decision> e)
        {
            sw.Stop();
            bool save = false;
            bool spacing = false;
            if (best != null)
            {
                if (e.Best.Fitness > best.Fitness) //individuum has a higher fitness
                    save = true;
                else if (e.Best.Fitness == best.Fitness) //Individuum has the same fitness
                {
                    for (int i = 0; i < e.Best.Data.Length; i++) //Check if data array are not-equal
                    {
                        if (e.Best.Data[i] != best.Data[i])
                        {
                            save = true; //not equal save
                            break;
                        }
                    }
                }
            }
            else
            {
                save = true;
                spacing = true;
            }
            
            if(save)
            {
                best = e.Best; //set new best
                
                StringBuilder sb = new StringBuilder();
                if(spacing)
                {
                    sb.Append(Environment.NewLine);
                    sb.Append("=============================================");
                    sb.Append(Environment.NewLine);
                    sb.Append("=============================================");
                    sb.Append(Environment.NewLine);
                    sb.Append("=========== NEW TRAININGS SESSION ===========");
                    sb.Append(Environment.NewLine);
                    sb.Append("=============================================");
                    sb.Append(Environment.NewLine);
                    sb.Append("=============================================");
                    sb.Append(Environment.NewLine);
                }
                sb.Append("Iteration: ");
                sb.Append(e.Iteration.ToString());
                sb.Append(Environment.NewLine);
                sb.Append("Best: ");
                sb.Append(e.Best.Fitness.ToString());
                sb.Append(Environment.NewLine);

                sb.Append("Duration: ");
                sb.Append(sw.ElapsedMilliseconds.ToString());
                sb.Append(Environment.NewLine);

                sb.Append("Primary weapon: ");
                sb.Append(level.PrimaryWeapons[e.Best.Data[0].Value].Name);
                sb.Append(Environment.NewLine);

                sb.Append("Primary suspressed: ");
                sb.Append(e.Best.Data[1].Value == 1 ? "Yes" : "No");
                sb.Append(Environment.NewLine);

                sb.Append("Secndary weapon:");
                sb.Append(level.SecondaryWeapons[e.Best.Data[2].Value].Name);
                sb.Append(Environment.NewLine);

                sb.Append("Secondary suspressed: ");
                sb.Append(e.Best.Data[3].Value == 1 ? "Yes" : "No");
                sb.Append(Environment.NewLine);

                sb.Append("Entry index: ");
                sb.Append(e.Best.Data[4].Value.ToString());
                sb.Append(" Entry Position: ");
                sb.Append(entryPositions[e.Best.Data[4].Value].ToString());
                sb.Append(Environment.NewLine);

                sb.Append("Exit index: ");
                sb.Append(e.Best.Data[5].Value.ToString());
                sb.Append(" Exit Position: ");
                sb.Append(entryPositions[e.Best.Data[5].Value].ToString());
                sb.Append(Environment.NewLine);
                sb.Append("===================================================");
                sb.Append(Environment.NewLine);

                File.AppendAllText("decisions.txt", sb.ToString());
            }
            Console.WriteLine("Iteration " + (e.Iteration + 1).ToString() + " completed: Best = " + e.Best.Fitness.ToString());
        }

        /// <summary>
        /// Executed by the function "CalculateFitness" from every DNA (individuum) 
        /// </summary>
        /// <param name="decisions">decisions of the individuum</param>
        /// <returns>Returns players health</returns>
        public int DoTraining(Genetics.Decision[] decisions)
        {
            Console.WriteLine("Game-Session started!");

            //Setd the "virtual" elapsed time between updates (high values (>16) can break collision detection and pathfinding)
            TimeSpan elapsedTime = new TimeSpan(0, 0, 0, 0, 1);
            GameTime gameTime = new GameTime(new TimeSpan(0), elapsedTime);

            SetupDecisions(decisions);
            ResetLevel();

            //game session
            GameEndState state = GameEndState.None;
            do
            {
                gameTime.TotalGameTime += elapsedTime;
                Update(gameTime);
                state = CheckGameState();

                //speed up or slow down simulation (can be used to render/spectate in realtime)
                if (gameMode == GameMode.Spectator)
                    Thread.Sleep(4);

            } while (state == GameEndState.None);

            //Set health to 1 if hostage was grabbed
            int health = Convert.ToInt32(Math.Round(GetPlayerHealth()));
            if (health == 0 && player.HostageGrabbed > 0)
                health = 1;

            //Output Trainings results
            DrawTrainingsResault(state, health);
            return health;
        }

        /// <summary>
        /// Setup decisions created by the genetic algorithm
        /// </summary>
        /// <param name="decisions"></param>
        private void SetupDecisions(Genetics.Decision[] decisions)
        {
            primaryWeaponName = level.PrimaryWeapons[decisions[0].Value].Name;
            primarySuspressed = Convert.ToBoolean(decisions[1].Value);
            secondaryWeaponName = level.SecondaryWeapons[decisions[2].Value].Name;
            secondarySuspressed = Convert.ToBoolean(decisions[3].Value);

            waypoints = new List<Vector2>();
            waypoints.Add(entryPositions[decisions[4].Value] + new Vector2(25, 25)); //entry

            waypoints.Add(entryPositions[decisions[5].Value] + new Vector2(25, 25)); //exit
            waypoints.Add(new Vector2(entryPositions[decisions[5].Value].X + 25, 0)); //rescue
        }

        public void ResetLevel()
        {
            entityManager = new EntityManager();
            EntityManager.ActivEntityManager = entityManager;
            level = new Level(levelPath, 50, this);
            enemies = null;
            
            playerIsDead = false;
            enmiesAreDead = false;
            level.Spawn();
            if (gameMode == GameMode.PlayAble)
            {
                player = new Player(level.PlayerInfo.Speed, level.PlayerInfo.RotationSpeed, true, loadedTextures["player"], 0, level.PlayerInfo.Rotation, Vector2.One, Color.White, "Player", new Vector2(level.PlayerInfo.ColumnIndex * 50, level.PlayerInfo.RowIndex * 50), true, true, level.PlayerInfo.TeamId);
            }
            else
            {
                float y = level.PlayerInfo.RowIndex * 50;
                float x = level.PlayerInfo.ColumnIndex * 50;
                if (waypoints != null)
                {
                    //waypoints add hostage
                    Entity[] hostages = EntityManager.ActivEntityManager.GetEntitiesByName("Hostage");
                    if (hostages != null)
                    {
                        for (int i = 0; i < hostages.Length; i++)
                            waypoints.Insert(1,(hostages[i] as Character).Center);
                    }

                    x = waypoints[0].X;
                    waypoints[waypoints.Count - 1] = new Vector2(waypoints[waypoints.Count - 1].X, y);
                }
                PlayerController ki = new PlayerController(level, waypoints, level.PlayerInfo.Speed, level.PlayerInfo.RotationSpeed, true, loadedTextures["player"], 0, level.PlayerInfo.Rotation, Vector2.One, Color.White, "Player", new Vector2(x, y), true, true, level.PlayerInfo.TeamId);
                player = ki;
            }

            if (primaryWeaponName == null && level.PrimaryWeapons.Length > 0)
                player.PickupWeapon(level.PrimaryWeapons[Helpers.Rand.Next(0, level.PrimaryWeapons.Length)]);
            else
                player.PickupWeapon(Level.GetWeapon(level.PrimaryWeapons, primaryWeaponName, primarySuspressed));

            if (secondaryWeaponName == null && level.SecondaryWeapons.Length > 0)
                player.PickupWeapon(level.SecondaryWeapons[Helpers.Rand.Next(0, level.SecondaryWeapons.Length)]);
            else
                player.PickupWeapon(Level.GetWeapon(level.SecondaryWeapons, secondaryWeaponName, secondarySuspressed));

            DamageAbleObject dmg = new DamageAbleObject(player, "health", 1000, 1000);
            player.AddComponent(dmg);
            dmg.OnDeath += Dmg_OnDeath;

            if (enemies == null)
            {
                enemies = entityManager.GetEntitiesByType<NPC>().ToList();
                for (int i = 0; i < enemies.Count; i++)
                {
                    DamageAbleObject dmgNPC = enemies[i].GetComponent<DamageAbleObject>();
                    dmgNPC.OnDeath += DmgNPC_OnDeath;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (level != null)
            {
                entityManager?.Update(gameTime);

                GameEndState state = CheckGameState();
                if (state != GameEndState.None)
                {
                    if (gameMode == GameMode.Training)
                    {
                        GameOver?.Invoke(this, new GameOverArgs(state, Convert.ToInt32(Math.Round(GetPlayerHealth()))));
                    }
                    else
                    {
                        GameOver?.Invoke(this, null);
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (level != null)
            {
                entityManager?.Draw(spriteBatch);
            }
        }

        private void DrawTrainingsResault(GameEndState gameEndState, int health)
        {
            Console.WriteLine("Game-Session ended => State: " + gameEndState.ToString() + ", PlayerHealth(Fitness): " + health.ToString());
        }

        private GameEndState CheckGameState()
        {
            if (playerIsDead)
                return GameEndState.PlayerDied;
            else if (enmiesAreDead)
                return GameEndState.AllEnemiesKilled;
            else if (player.HostagesRescued)
                return GameEndState.HostageRescued;

            return GameEndState.None;
        }

        private void Dmg_OnDeath(object sender, HealthChangedArgs e)
        {
            playerIsDead = true;
        }

        public override void LoadContent(ContentManager content)
        {
            content.RootDirectory = "Content";
            loadedTextures = new Dictionary<string, Texture2D>();

            string[] files = Directory.GetFiles("Content/Textures");
            for (int i = 0; i < files.Length; i++)
            {
                string fileName = Path.GetFileNameWithoutExtension(files[i]);
                loadedTextures.Add(fileName, content.Load<Texture2D>("Textures\\" + fileName));
            }

            ResetLevel();

            switch (gameMode)
            {
                case GameMode.Spectator:
                    {
                        Entity[] entries = EntityManager.ActivEntityManager.GetEntitiesByName("Entries");
                        entryPositions = new Vector2[entries.Length];
                        for (int i = 0; i < entries.Length; i++)
                        {
                            entryPositions[i] = entries[i].Position;
                        }
                        balacing = new Genetics.BalancingGeneticAlgorithm(level.PrimaryWeapons, level.SecondaryWeapons, entryPositions, this, 30, 0.1f, 0.8f, 0.1f, 0.1f, 1, iterations);
                        thread = new Thread(StartTraining);
                        thread.IsBackground = true;
                        thread.Start();
                    }
                    break;
                case GameMode.Training:
                    {
                        Entity[] entries = EntityManager.ActivEntityManager.GetEntitiesByName("Entries");
                        entryPositions = new Vector2[entries.Length];
                        for (int i = 0; i < entries.Length; i++)
                        {
                            entryPositions[i] = entries[i].Position;
                        }
                        balacing = new Genetics.BalancingGeneticAlgorithm(level.PrimaryWeapons, level.SecondaryWeapons, entryPositions, this, 30, 0.1f, 0.8f, 0.1f, 0.1f, 1, iterations);
                        thread = new Thread(StartTraining);
                        thread.IsBackground = true;
                        thread.Start();
                    }
                    break;
            }
        }

        private float GetPlayerHealth()
        {
            float health = 0f;
            if (player != null)
            {
                DamageAbleObject healthContainer = player.GetComponent<DamageAbleObject>();
                if (healthContainer != null && healthContainer.Health > 0)
                    health = healthContainer.Health;
            }
            return health;
        }
    }
}
