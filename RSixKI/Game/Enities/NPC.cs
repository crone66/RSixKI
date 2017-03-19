using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RSixKI
{
    public class NPC : Character
    {
        private const float padding = 3f;
        private const float viewDistance = 500f;
        private const float fov = 45f;

        private Player player;
        private CollidAble[] collidables;
        private Vector2 lastKnownPosition;
        private float elapsedTimeSinceContact = float.MaxValue;
        private DamageAbleObject health;

        private bool attacked;
        private float elasedTimeSinceAttacked = float.MaxValue;
        private float defaultRotation;

        private bool hasCover;
        private bool covered;

        public bool Covered
        {
            get
            {
                return covered;
            }

            set
            {
                covered = value;
            }
        }

        public NPC(float speed, float rotationSpeed, bool blocking, Texture2D texture, int layer, float rotation, Vector2 scale, Color color, string name, Vector2 position, bool isVisable = true, bool isActiv = true, int teamId = -1) : base(speed, rotationSpeed,blocking, texture, layer, rotation, scale, color, name, position, isVisable, isActiv, teamId)
        {
            defaultRotation = rotation;
            OnTriggerEnter += NPC_OnTriggerEnter;
        }

        public NPC(float speed, float rotationSpeed, bool blocking, Texture2D texture, int layer, float rotation, Vector2 scale, Color color, string name, Vector2 position, IEnumerable<EntityComponent> components, bool isVisable = true, bool isActiv = true, int teamId = -1) : base(speed, rotationSpeed, blocking, texture, layer, rotation, scale, color, name, position, components, isVisable,isActiv, teamId)
        {
            defaultRotation = rotation;
            OnTriggerEnter += NPC_OnTriggerEnter;
        }

        public override void Update(GameTime gameTime)
        {
            if (player == null || !player.IsActiv)
            {
                player = EntityManager.ActivEntityManager.GetEntityByType<Player>();
            }

            if (collidables == null)
            {
                collidables = EntityManager.ActivEntityManager.GetEntitiesByType<CollidAble>();
            }

            if (health == null)
            {
                health = GetComponent<DamageAbleObject>();
                if (health != null)
                {
                    health.OnHealthChanging += Health_OnHealthChanging;
                    health.OnHealthChanged += Health_OnHealthChanged;
                }
            }

            if (attacked)
            {
                elasedTimeSinceAttacked += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (elasedTimeSinceAttacked > 5000f)
                    attacked = false;
            }

            Think((float)gameTime.ElapsedGameTime.TotalMilliseconds);
            base.Update(gameTime);
        }

        private void Health_OnHealthChanging(object sender, HealthChangingArgs e)
        {
            Projectile projectile = e.HealthChanger as Projectile;
            if(covered && projectile != null && projectile.CrossedCover)
            {
                e.Cancel = true;
            }
        }

        private void Health_OnHealthChanged(object sender, HealthChangedArgs e)
        {
            if(e.HealthChanger is Projectile)
                hasCover = (e.HealthChanger as Projectile).CrossedCover;

            attacked = true;
            elapsedTimeSinceContact = 0;
        }

        private void Think(float elapsedMs)
        {
            if (player != null)
            {
                CheckCover();

                if (SearchPlayer())
                {
                    if (inHand.AmmoInMag > 0)
                    {
                        if (!covered)
                        {
                            if (Aim(player.Position))
                            {
                                Weapon.FireState state = Attack(player);
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
                    if (elapsedTimeSinceContact > 1000f && !inHand.Reloading && inHand.AmmoInMag < inHand.MaxMagSize)
                        Reload();

                    if (elapsedTimeSinceContact > 5000f)
                    {
                        if(Rotation != defaultRotation)
                            Aim(Position + Helpers.ConvertToDirection(Position, defaultRotation));
                    }
                    else
                    {
                        Aim(lastKnownPosition);
                    }
                }
            }
        }

        private bool SearchPlayer()
        {
            if (Helpers.IsInView(Center, Rotation, fov, viewDistance, player.Center))
            {
                float playerDistance = Vector2.Distance(Center, player.Center);
                Vector2 direction = player.Center - Center;
                direction.Normalize();

                bool hidden = false;
                Ray ray = new Ray(new Vector3(Center, 0), new Vector3(direction, 0));
                for (int i = 0; i < collidables.Length; i++)
                {
                    if (collidables[i] != player && collidables[i] != this)
                    {
                        float? distance = Helpers.Raycast2D(collidables[i].CollisionBox, ray);
                        if (distance.HasValue && distance < playerDistance)
                        {
                            if (collidables[i].Name == "Cover")
                                hasCover = true;

                            hidden = true;
                            break;
                        }
                    }
                }

                if(hasCover)
                    CleanUpCollidables();

                if (!hidden)
                {
                    lastKnownPosition = player.Center;
                    return true;
                }
            }

            return false;
        }

        private void CleanUpCollidables()
        {
            List<CollidAble> tempCollidables = new List<CollidAble>();
            for (int i = 0; i < collidables.Length; i++)
            {
                if (collidables[i].Name != "Cover")
                    tempCollidables.Add(collidables[i]);
            }
            collidables = tempCollidables.ToArray();
        }

        private bool Aim(Vector2 targetPosition)
        {
            Vector2 direction = Position - targetPosition;
            float targetAngle = Helpers.ConvertToAngle(direction);

            Helpers.RotationDirection rotDirection = Helpers.GetRotationDirection(targetAngle, Rotation, padding);
            switch(rotDirection)
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

            return false;
        }

        private void CheckCover()
        {
            if (hasCover)
            {
                if (!covered)
                {
                    covered = attacked && elasedTimeSinceAttacked < 3000f;
                }
                else
                {
                    if ((attacked && elasedTimeSinceAttacked <= 0) || elasedTimeSinceAttacked >= 2000f || !attacked)
                    {
                        covered = false;
                    }
                }
            }
        }

        private void NPC_OnTriggerEnter(object sender, CollisionArgs e)
        {
            if(e.CollisionObject.Name == "Noise")
            {
                if (e.CollisionObject.TeamId != teamId)
                {
                    bool hidden = false;
                    float distanceToNoise = Vector2.Distance(Center, e.CollisionObject.Center);
                    Vector2 direction = Center - e.CollisionObject.Center;
                    direction.Normalize();
                    Ray ray = new Ray(new Vector3(Center, 0), new Vector3(direction, 0));
                    for (int i = 0; i < collidables.Length; i++)
                    {
                        if (collidables[i] != player && collidables[i] != this)
                        {
                            float? distance = Helpers.Raycast2D(collidables[i].CollisionBox, ray);
                            if (distance.HasValue && distance < distanceToNoise)
                            {
                                hidden = true;
                                break;
                            }
                        }
                    }

                    if (!hidden)
                    {
                        lastKnownPosition = e.CollisionObject.Center;
                        elapsedTimeSinceContact = 0f;
                    }
                }
            }
        }

    }
}
