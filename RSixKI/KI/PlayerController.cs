
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RSixKI
{
    public class PlayerController : Player
    {
        private const float padding = 3f;
        private const float viewDistance = 500f;
        private const float fov = 45f;

        private List<NPC> npcs;
        private List<NPC> npcsInView;
        private Vector2 lastKnownPosition;
        private CollidAble[] collidables;
        private float elapsedTimeSinceContact = float.MaxValue;
        private DamageAbleObject health;

        private bool attacked;
        private float elasedTimeSinceAttacked = float.MaxValue;
        private Level level;
        private List<Vector2> waypoints;

        public PlayerController(Level level, List<Vector2> waypoints, float speed, float rotationSpeed, bool blocking, Texture2D texture, int layer, float rotation, Vector2 scale, Color color, string name, Vector2 position, bool isVisable = true, bool isActiv = true, int teamId = -1) : base(speed, rotationSpeed, blocking, texture, layer, rotation, scale, color, name, position, isVisable, isActiv, teamId)
        {
            this.level = level;
            this.waypoints = waypoints;
            OnTriggerEnter += KI_OnTriggerEnter;
            npcsInView = new List<NPC>();
        }

        public PlayerController(Level level, List<Vector2> waypoints, float speed, float rotationSpeed, bool blocking, Texture2D texture, int layer, float rotation, Vector2 scale, Color color, string name, Vector2 position, IEnumerable<EntityComponent> components, bool isVisable = true, bool isActiv = true, int teamId = -1) : base(speed, rotationSpeed, blocking, texture, layer, rotation, scale, color, name, position, components, isVisable, isActiv, teamId)
        {
            this.level = level;
            this.waypoints = waypoints;
            OnTriggerEnter += KI_OnTriggerEnter;
            npcsInView = new List<NPC>();
        }

        public override void Update(GameTime gameTime)
        {
            if (npcs == null)
            {
                npcs = new List<NPC>(EntityManager.ActivEntityManager.GetEntitiesByType<NPC>());
                for (int i = 0; i < npcs.Count; i++)
                {
                    DamageAbleObject dmg = npcs[i].GetComponent<DamageAbleObject>();
                    if(dmg != null)
                    {
                        dmg.OnDeath += Dmg_OnDeath;
                    }
                }
            }

            if (collidables == null)
            {
                collidables = EntityManager.ActivEntityManager.GetEntitiesByType<CollidAble>();
            }

            if (health == null)
            {
                health = GetComponent<DamageAbleObject>();
                if (health != null)
                    health.OnHealthChanged += Health_OnHealthChanged;
            }

            if (attacked)
            {
                elasedTimeSinceAttacked += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (elasedTimeSinceAttacked > 5000f)
                    attacked = false;
            }

            Think((float)gameTime.ElapsedGameTime.TotalMilliseconds);
            BaseUpdate(gameTime);
        }

        private void Dmg_OnDeath(object sender, HealthChangedArgs e)
        {
            DamageAbleObject dmg = sender as DamageAbleObject;
            npcs.Remove(dmg.Parent as NPC);
            elapsedTimeSinceContact = 10000;
            attacked = false;
            elasedTimeSinceAttacked = 10000f;
        }

        private void Health_OnHealthChanged(object sender, HealthChangedArgs e)
        {
            attacked = true;
            elasedTimeSinceAttacked = 0f;
        }

        private float elapsedTimeDoingNothing = 0;
        private Vector2 prevPosition;
        private void Think(float elapsedMs)
        {
            if (npcs != null)
            {
                if (SearchPlayer())
                {
                    if (inHand.AmmoInMag > 0)
                    {
                        if (Aim(npcsInView[0].Center, Center))
                        {
                            if(inHand.Reloading)
                            {
                                if (inHand.Slot == Weapon.WeaponSlot.Primary)
                                    SwitchWeapon(Weapon.WeaponSlot.Secondary);
                                else
                                    SwitchWeapon(Weapon.WeaponSlot.Primary);
                            }

                            if (inHand.AmmoInMag > 0 && !inHand.Reloading)
                            {
                                Weapon.FireState state = Attack(npcsInView[0]);
                                switch (state)
                                {
                                    case Weapon.FireState.OutOfAmmo: //Stop thinking?
                                        Destroy();
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!inHand.Reloading)
                        {
                            Reload();
                        }
                    }
                }
                else
                {
                    elapsedTimeSinceContact += elapsedMs;
                    if (elapsedTimeSinceContact > 1000f && !inHand.Reloading && inHand.AmmoInMag / inHand.MaxMagSize < 0.2f)
                        Reload();

                    if (elapsedTimeSinceContact < 2000f)
                        Aim(lastKnownPosition, Center);
                    else
                    {
                        Move();
                        Aim(Position + direction, Position, true);
                    }
                }

                if (Vector2.Distance(prevPosition, Position) > 5)
                    elapsedTimeDoingNothing = 0;

                elapsedTimeDoingNothing += elapsedMs;
                if (elapsedTimeDoingNothing > 10000)
                {
                    health.ChangeHealth(this, -health.MaxHealth);
                }
                prevPosition = Position;
            }
        }

        private Node path;
        private void Move()
        {
            if (waypoints.Count == 0)
                return;

            if (Vector2.Distance(waypoints[0], Center) < 10f)
            {
                Position = waypoints[0] - new Vector2(25, 25);
                bool waypoint = false;
                for (int i = 0; i < hostages.Count; i++)
                {
                    if((hostages[i] as Character).Center == waypoints[0])
                    {
                        waypoint = true;
                        break;
                    }
                }
                if (!waypoint)
                {
                    path = null;
                    waypoints.RemoveAt(0);
                }
            }

            if (waypoints.Count > 0)
            {
                if(path == null)
                    path = level.PathFinding.Search(Center, waypoints[0]);
                else
                {
                    Node currentNode = path;
                    Node prvNode = path;
                    while (currentNode.Parent != null)
                    {
                        prvNode = currentNode;
                        currentNode = currentNode.Parent;
                    }

                    if (Vector2.Distance(currentNode.position, Center) < 10f)
                    {
                        Position = currentNode.position - new Vector2(25, 25);
                        if(currentNode == prvNode)
                        {
                            direction = waypoints[0] - Center;
                            direction.Normalize();
                        }
                        else
                            prvNode.Parent = null;
                    }
                    else
                    {
                        direction = currentNode.position - Center;
                        direction.Normalize();
                    }
                }
            }
        }

        private bool SearchPlayer()
        {
            npcsInView.Clear();
            for (int i = 0; i < npcs.Count; i++)
            {
                if (Helpers.IsInView(Center, Rotation, fov, viewDistance, npcs[i].Center))
                {
                    float playerDistance = Vector2.Distance(Center, npcs[i].Center);
                    Vector2 direction = npcs[i].Center - Center;
                    direction.Normalize();

                    bool hidden = false;
                    Ray ray = new Ray(new Vector3(Center, 0), new Vector3(direction, 0));
                    for (int j = 0; j < collidables.Length; j++)
                    {
                        if (collidables[j] != npcs[i] && collidables[j] != this && (collidables[j].Name != "Cover" || npcs[i].Covered))
                        {
                            float? distance = Helpers.Raycast2D(collidables[j].CollisionBox, ray);
                            if (distance.HasValue && distance < playerDistance)
                            {
                                hidden = true;
                                break;
                            }
                        }
                    }

                    if (!hidden)
                    {
                        lastKnownPosition = npcs[i].Center;
                        npcsInView.Add(npcs[i]);
                    }
                }
            }

            return npcsInView.Count > 0;
        }

        private bool Aim(Vector2 targetPosition, Vector2 position, bool snap = false)
        {
            Vector2 direction = position - targetPosition;
            float targetAngle = Helpers.ConvertToAngle(direction);

            if (!snap)
            {
                Helpers.RotationDirection rotDirection = Helpers.GetRotationDirection(targetAngle, Rotation, padding);
                switch (rotDirection)
                {
                    case Helpers.RotationDirection.Left:
                        RotateLeft();
                        break;
                    case Helpers.RotationDirection.Right:
                        RotateRight();
                        break;
                    case Helpers.RotationDirection.None:
                        Rotation = targetAngle;
                        return true;
                }
            }
            else
            {
                Rotation = targetAngle;
                return true;
            }
            return false;
        }

        private void KI_OnTriggerEnter(object sender, CollisionArgs e)
        {
            if (e.CollisionObject.Name == "Noise")
            {
                if (e.CollisionObject.TeamId != teamId)
                    lastKnownPosition = e.CollisionObject.Center;

                elapsedTimeSinceContact = 0f;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (path != null)
            {
                Node node = path;
                while (node != null && node.Parent != null)
                {
                    spriteBatch.Draw(Game1.PathDebug, node.position - new Vector2(25, 25), Color.White);
                    node = node.Parent;
                }
            }
            base.Draw(spriteBatch);
        }
    }
}
