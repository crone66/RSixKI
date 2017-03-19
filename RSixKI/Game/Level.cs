using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace RSixKI
{
    public class Level
    {
        private List<Entity> tiles;
        private int columnCount;
        private int rowCount;
        private int tileSize;
        private LevelContainer levelInformation;
        private Weapon[] weapons;
        private Weapon[] primaryWeapons;
        private Weapon[] secondaryWeapons;
        private CharacterInformation playerInfo;
        private GameScene gameScene;

        private AStar pathFinding;

        public CharacterInformation PlayerInfo
        {
            get
            {
                return playerInfo;
            }
        }

        public LevelContainer LevelInformation
        {
            get
            {
                return levelInformation;
            }
        }

        public Weapon[] Weapons
        {
            get { return weapons; }
        }

        public Weapon[] PrimaryWeapons
        {
            get { return primaryWeapons; }
        }

        public Weapon[] SecondaryWeapons
        {
            get { return secondaryWeapons; }
        }

        public AStar PathFinding
        {
            get { return pathFinding; }
        }

        public Level(int columnCount, int rowCount, int tileSize, List<Entity> tiles, LevelContainer levelInformation, GameScene gameScene)
        {
            this.columnCount = columnCount;
            this.rowCount = rowCount;
            this.tiles = tiles;
            this.tileSize = tileSize;
            this.levelInformation = levelInformation;
            this.gameScene = gameScene;
            Init(columnCount, rowCount, tileSize);
            SetupPathFinding();
            LoadWeapons(gameScene.LoadedTextures["projectile"]);
        }

        public Level(string path, int tileSize, GameScene gameScene)
        {
            columnCount = -1;
            rowCount = -1;
            tiles = null;
            this.tileSize = tileSize;
            this.gameScene = gameScene;
            if (!LoadLevel(path, tileSize, out columnCount, out rowCount, out tiles, gameScene.LoadedTextures, out levelInformation))
            {
                throw new Exception("Couldn't load level " + path + " !");
            }
            else
            {
                SetupPathFinding();
                LoadWeapons(gameScene.LoadedTextures["projectile"]);
            }
        }

        private void SetupPathFinding()
        {
            pathFinding = new AStar();
            for (int i = 0; i < tiles.Count; i++)
                pathFinding.AddNode((tiles[i] as DrawAble).Center);

            for (int i = 0; i < pathFinding.Nodes.Count; i++)
            {
                List<Node> neighbors = new List<Node>();
                for (int j = 0; j < pathFinding.Nodes.Count; j++)
                {
                    if (i == j)
                        continue;

                    float x = Math.Abs(pathFinding.Nodes[i].position.X - pathFinding.Nodes[j].position.X);
                    float y = Math.Abs(pathFinding.Nodes[i].position.Y - pathFinding.Nodes[j].position.Y);
                    if ((x <= tileSize && y <= tileSize) && !(x == tileSize && y == tileSize))
                    {
                        neighbors.Add(pathFinding.Nodes[j]);
                    }
                }
                pathFinding.Nodes[i].Neighbors = neighbors.ToArray();
            }
        }

        private void LoadWeapons(Texture2D projectile)
        {
            weapons = LoadWeapons("Content/Weapons/weapons.xml", projectile);
            List<Weapon> primary = new List<Weapon>();
            List<Weapon> secondary = new List<Weapon>();
            for (int i = 0; i < weapons.Length; i++)
            {
                if (weapons[i].Slot == Weapon.WeaponSlot.Primary)
                    primary.Add(weapons[i]);
                else if (weapons[i].Slot == Weapon.WeaponSlot.Secondary)
                    secondary.Add(weapons[i]);
            }
            primaryWeapons = primary.ToArray();
            secondaryWeapons = secondary.ToArray();
        }

        public void Spawn()
        {
            //Load Npcs
            foreach (CharacterInformation item in levelInformation.Characters)
            {
                if (item.Type == CharacterType.Enemy)
                {
                    NPC npc = new NPC(item.Speed, item.RotationSpeed, true, gameScene.LoadedTextures["npc"], 0, item.Rotation, Vector2.One, Color.White, item.Name, new Vector2(item.ColumnIndex * tileSize, item.RowIndex * tileSize), true, true, item.TeamId);
                    npc.AddComponent(new DamageAbleObject(npc, "health", item.Health, item.Health));
                    for (int i = 0; i < item.WeaponNames.Length; i++)
                    {
                        npc.PickupWeapon(GetWeapon(weapons, item.WeaponNames[i]));
                    }
                }
                else if (item.Type == CharacterType.Hostage)
                {
                    Character character = new Character(item.Speed, item.RotationSpeed, true, gameScene.LoadedTextures["hostage"], 0, item.Rotation, Vector2.One, Color.White, item.Name, new Vector2(item.ColumnIndex * tileSize, item.RowIndex * tileSize), true, true, item.TeamId);
                }
                else if (item.Type == CharacterType.Player)
                {
                    playerInfo = item;
                }
            }
        }

        private static void Init(int columnCount, int rowCount, int tileSize)
        {
            CollisionGrid[] grids = new CollisionGrid[] { new CollisionGrid(new Rectangle(0, 0, (columnCount / 2) * tileSize, (rowCount / 2) * tileSize)),
                new CollisionGrid(new Rectangle((columnCount / 2) * tileSize, (rowCount / 2) * tileSize, (columnCount / 2) * tileSize, (rowCount / 2) * tileSize)),
                new CollisionGrid(new Rectangle((columnCount / 2) * tileSize, 0, (columnCount / 2) * tileSize, (rowCount / 2) * tileSize)),
                new CollisionGrid(new Rectangle(0, (rowCount / 2) * tileSize, (columnCount / 2) * tileSize, (rowCount / 2) * tileSize))
            };

            new CollisionDetection(grids);
        }

        private static Weapon[] LoadWeapons(string path, Texture2D texture)
        {
            if (File.Exists(path))
            {
                object result = null;
                XmlSerializer xml = new XmlSerializer(typeof(WeaponContainer));
                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    result = xml.Deserialize(stream);
                }

                if(result != null)
                {
                    WeaponContainer container = (WeaponContainer)result;
                    Projectile[] projectiles = new Projectile[container.Projectiles.Length];

                    for (int i = 0; i < container.Projectiles.Length; i++)
                    {
                        ProjectileInformation proji = container.Projectiles[i];
                        projectiles[i] = new Projectile(proji.Damage, proji.SuspressedDamage, proji.DamageReductionPerUnit, proji.Speed, proji.LifeTime, Game1.CurrentGame.GameMode == GameScene.GameMode.PlayAble ? proji.RayCastShot : true, false, texture, 0, 0, Vector2.One, Color.White, proji.Name, Vector2.Zero);
                    }

                    Weapon[] weapons = new Weapon[container.Weapons.Length];
                    for (int i = 0; i < container.Weapons.Length; i++)
                    {
                        WeaponInformation weaponInfo = container.Weapons[i];
                        weapons[i] = new Weapon(weaponInfo.Name, Vector2.Zero, weaponInfo.EquipmentType, weaponInfo.Firerate, SearchProjectile(projectiles, weaponInfo.ProjectileName), weaponInfo.AmmoType, weaponInfo.Slot, weaponInfo.ClipCount * weaponInfo.ClipSize, weaponInfo.ClipSize, weaponInfo.ReloadTime, weaponInfo.Mobility);
                           
                    }

                    return weapons;
                }
            }

            return null;
        }

        public static Weapon GetWeapon(Weapon[] weapons, string name, bool suspressed = false)
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                if (weapons[i].Name == name)
                {
                    Weapon weapon = weapons[i].Clone() as Weapon;
                    weapon.Suspressed = suspressed;
                    return weapon;
                }
            }

            return null;
        }

        private static Projectile SearchProjectile(Projectile[] projectiles, string name)
        {
            for (int i = 0; i < projectiles.Length; i++)
            {
                if (projectiles[i].Name == name)
                    return projectiles[i];
            }
            return null;
        }

        public static bool LoadLevel(string path, int tileSize, out int columnCount, out int rowCount, out List<Entity> tiles, Dictionary<string, Texture2D> loadedTextures, out LevelContainer level)
        {
            columnCount = -1;
            rowCount = -1;
            tiles = new List<Entity>();
            level = default(LevelContainer);
            if(File.Exists(path))
            {
                object result = null;
                XmlSerializer xml = new XmlSerializer(typeof(LevelContainer));
                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    result = xml.Deserialize(stream);
                }

                if (result != null)
                {
                    level = (LevelContainer)result;

                    string[] lines = level.MapLayout;
                    rowCount = lines.Length;
                    for (int y = 0; y < rowCount; y++)
                    {
                        columnCount = lines[y].Length;

                        if (y == 0)
                            Init(columnCount, rowCount, tileSize);

                        for (int x = 0; x < columnCount; x++)
                        {
                            int value = -1;
                            if (int.TryParse(lines[y][x].ToString(), out value))
                            {
                                if (loadedTextures.ContainsKey(value.ToString()))
                                {
                                    if (value == 0)
                                    {
                                        tiles.Add(new VisableEntity(loadedTextures[value.ToString()], 1, 0, Vector2.One, Color.White, value.ToString(), new Vector2(x * tileSize, y * tileSize)));
                                    }
                                    else if (value == 1)
                                    {
                                        new InteractivObject(true, loadedTextures[value.ToString()], 1, 0, Vector2.One, Color.White, value.ToString(), new Vector2(x * tileSize, y * tileSize));
                                    }
                                    else if(value == 2)
                                    {
                                        new InteractivObject(true, loadedTextures[value.ToString()], 1, 0, Vector2.One, Color.White, "Cover", new Vector2(x * tileSize, y * tileSize));
                                    }
                                    else if (value < 6)
                                    {
                                        //Interactive
                                        tiles.Add(new VisableEntity(loadedTextures[value.ToString()], 1, 0, Vector2.One, Color.White, "Entries", new Vector2(x * tileSize, y * tileSize), true, true));
                                    }
                                    else if (value == 6)
                                    {
                                        tiles.Add(new InteractivObject(false, loadedTextures["0"], 1, 0, Vector2.One, Color.White, "RescueZone", new Vector2(x * tileSize, y * tileSize)));
                                        /*Projectile projectile = new Projectile()
                                        Player p = new Player(true, Game1.LoadedTextures[value.ToString()], 0, 0, Vector2.One, Color.White, value.ToString(), new Vector2(x * tileSize, y * tileSize));
                                        p.PickupWeapon(new Weapon("Primary", , Equipment.EquipmentType.Useable, 100, null, 1, false, 1))*/
                                    }
                                    else
                                    {
                                        //Enmeies, Player, Hostage, RescueZone
                                        tiles.Add(new VisableEntity(loadedTextures[value.ToString()], 1, 0, Vector2.One, Color.White, value.ToString(), new Vector2(x * tileSize, y * tileSize)));
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Couldn't found asset: " + value.ToString());
                                }
                            }
                        }
                    }
                }
                //Load File
                /*
                 * 0 = Empty field
                 * 1 = Wall
                 * 2 = Obstacale
                 * 3 = Door
                 * 4 = Breakable Window 
                 * 5 = Breakable Wall
                 * 6 = RescueZone
                 * 7 = Enemy
                 * 8 = Hostage
                 * 9 = Player
                 */
                return true;
            }
            return false;
        }
    }
}
