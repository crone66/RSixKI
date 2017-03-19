using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace RSixKI
{
    public abstract class CollidAble : DrawAble
    {
        private Rectangle collisionBox;
        private bool blocking;
        private CollisionGrid[] grids;
        private CollidAble[] previouseCollisions;

        public event EventHandler<CollisionArgs> OnCollision;
        public event EventHandler<CollisionArgs> OnCollisionEnter;
        public event EventHandler<CollisionArgs> OnCollisionExit;
        public event EventHandler<CollisionArgs> OnTrigger;
        public event EventHandler<CollisionArgs> OnTriggerEnter;
        public event EventHandler<CollisionArgs> OnTriggerExit;

        public Rectangle CollisionBox
        { 
            get
            {
                return collisionBox;
            }

            set
            {
                collisionBox = value;
            }
        }

        public bool Blocking
        {
            get { return blocking; }
        }

        public CollisionGrid[] Grids
        {
            get
            {
                return grids;
            }
            set
            {
                grids = value;
            }
        }

        public CollidAble[] PreviouseCollisions
        {
            get
            {
                return previouseCollisions;
            }
        }

        public CollidAble(bool blocking, Texture2D texture, int layer, float rotation, Vector2 scale, Color color, string name, Vector2 position, bool isVisable = true, bool isActiv = true, int teamId = -1) : base(texture, layer, rotation, scale, color, name, position, isVisable, isActiv, teamId)
        {
            OnDestroyed += CollidAble_OnDestroyed;
            OnSpawned += CollidAble_OnSpawned;
            OnMoved += CollidAble_OnMoved;
            previouseCollisions = new CollidAble[0];
            collisionBox = new Rectangle(0, 0, texture.Width, texture.Height);
            UpdateRectangle(position);
            this.blocking = blocking;
        }

        public CollidAble(bool blocking, Texture2D texture, int width, int height, int layer, float rotation, Vector2 scale, Color color, string name, Vector2 position, bool isVisable = true, bool isActiv = true, int teamId = -1) : base(texture, layer, rotation, new Vector2(width / 2, height / 2), scale, color, name, position, isVisable, isActiv, teamId)
        {
            OnDestroyed += CollidAble_OnDestroyed;
            OnSpawned += CollidAble_OnSpawned;
            OnMoved += CollidAble_OnMoved;
            previouseCollisions = new CollidAble[0];
            collisionBox = new Rectangle(0, 0, width, height);
            UpdateRectangle(position);
            this.blocking = blocking;
        }


        public CollidAble(bool blocking, Texture2D texture, int layer, float rotation, Vector2 scale, Color color, string name, Vector2 position, IEnumerable<EntityComponent> components, bool isVisable = true, bool isActiv = true, int teamId = -1) : base(texture, layer, rotation, scale, color, name, position, components, isVisable, isActiv, teamId)
        {
            OnDestroyed += CollidAble_OnDestroyed;
            OnSpawned += CollidAble_OnSpawned;
            OnMoved += CollidAble_OnMoved;
            previouseCollisions = new CollidAble[0];
            collisionBox = new Rectangle(0, 0, texture.Width, texture.Height);
            UpdateRectangle(position);
            this.blocking = blocking;
        }

        public CollidAble(bool blocking, Texture2D texture, int width, int height, int layer, float rotation, Vector2 scale, Color color, string name, Vector2 position, IEnumerable<EntityComponent> components, bool isVisable = true, bool isActiv = true, int teamId = -1) : base(texture, layer, rotation, scale, color, name, position, components, isVisable, isActiv, teamId)
        {
            OnDestroyed += CollidAble_OnDestroyed;
            OnSpawned += CollidAble_OnSpawned;
            OnMoved += CollidAble_OnMoved;
            previouseCollisions = new CollidAble[0];
            collisionBox = new Rectangle(0, 0, width, height);
            UpdateRectangle(position);
            this.blocking = blocking;
        }


        private void CollidAble_OnSpawned(object sender, EventArgs e)
        {
            CollisionDetection.ActivCollisionDetection.SignIn(this);
        }

        private void CollidAble_OnDestroyed(object sender, EventArgs e)
        {
            CollisionDetection.ActivCollisionDetection.SignOut(this);
            grids = null;
            previouseCollisions = null;
        }

        private void CollidAble_OnMoved(object sender, MovedArgs e)
        {
            UpdateRectangle(e.Position);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void DoCollisionCheck(bool triggered = false, CollidAble collidable = null)
        {
            if (IsActiv)
            {
                List<CollidAble> collisions;
                List<int> alreadyExists = new List<int>();
                if (!triggered)
                {
                    grids = CollisionDetection.ActivCollisionDetection.UpdateGrids(this);
                    collisions = CollisionDetection.ActivCollisionDetection.CheckCollision(this);
                }
                else
                {
                    collisions = new List<CollidAble>();
                    collisions.Add(collidable);
                }

                if (OnCollisionExit != null || OnTriggerExit != null)
                {
                    for (int i = 0; i < previouseCollisions.Length; i++)
                    {
                        if (!collisions.Contains(previouseCollisions[i]))
                        {
                            if (blocking && previouseCollisions[i].blocking)
                            {
                                OnCollisionExit?.Invoke(previouseCollisions[i], new CollisionArgs(previouseCollisions[i]));
                                if (!triggered)
                                    collisions[i].DoCollisionCheck(true, this);
                            }
                            else
                            {
                                OnTriggerExit?.Invoke(previouseCollisions[i], new CollisionArgs(previouseCollisions[i]));
                                if (!triggered)
                                    collisions[i].DoCollisionCheck(true, this);
                            }
                        }
                        else
                        {
                            alreadyExists.Add(i);
                        }
                    }
                }

                for (int i = 0; i < collisions.Count; i++)
                {
                    if (!alreadyExists.Contains(i))
                    {
                        if (blocking && collisions[i].blocking)
                        {
                            OnCollisionEnter?.Invoke(collisions[i], new CollisionArgs(collisions[i]));
                            if (!triggered)
                                collisions[i].DoCollisionCheck(true, this);
                        }
                        else
                        {
                            OnTriggerEnter?.Invoke(collisions[i], new CollisionArgs(collisions[i]));
                            if (!triggered)
                                collisions[i].DoCollisionCheck(true, this);
                        }
                    }
                    else
                    {
                        if (blocking && collisions[i].blocking)
                        {
                            OnCollision?.Invoke(collisions[i], new CollisionArgs(collisions[i]));
                            if (!triggered)
                                collisions[i].DoCollisionCheck(true, this);
                        }
                        else
                        {
                            OnTrigger?.Invoke(collisions[i], new CollisionArgs(collisions[i]));
                            if (!triggered)
                                collisions[i].DoCollisionCheck(true, this);
                        }
                    }
                }
            }
        }

        public void UpdateRectangle(Vector2 position)
        {
            collisionBox = new Rectangle(Convert.ToInt32(position.X), Convert.ToInt32(position.Y), collisionBox.Width, collisionBox.Height);
            DoCollisionCheck();
        }

        public abstract override object Clone();
    }
}
