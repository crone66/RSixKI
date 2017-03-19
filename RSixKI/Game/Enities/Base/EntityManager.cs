using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace RSixKI
{
    public class EntityManager
    {
        public static EntityManager ActivEntityManager;

        private List<Entity> entities;
        private List<DrawAble> drawAbleEntities;
        private List<CollidAble> collidAbleEntities;

        public EntityManager()
        {
            if (ActivEntityManager == null)
                ActivEntityManager = this;

            entities = new List<Entity>();
            drawAbleEntities = new List<DrawAble>();
            collidAbleEntities = new List<CollidAble>();
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                if(entities[i].IsActiv)
                    entities[i].Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront);
            for (int i = 0; i < drawAbleEntities.Count; i++)
            {
                drawAbleEntities[i].Draw(spriteBatch);
            }
            spriteBatch.End();
        }

        public void SignIn(Entity entity)
        {
            if (entity != null)
            {
                entity.OnActivStateChanged += Entity_OnActivStateChanged;
              
                if(entity.IsActiv)
                    entities.Add(entity);

                if (entity is CollidAble)
                {
                    if (entity.IsActiv)
                    {
                        CollidAble collidableEnt = entity as CollidAble;
                        collidAbleEntities.Add(collidableEnt);
                        CollisionDetection.ActivCollisionDetection.SignIn(collidableEnt);
                    }
                }

                if (entity is DrawAble)
                {
                    DrawAble drawAble = entity as DrawAble;
                    drawAble.OnVisabillityChanged += DrawAble_OnVisabillityChanged;
                    if (drawAble.IsVisable)
                        drawAbleEntities.Add(drawAble);
                }
            }
        }

        private void DrawAble_OnVisabillityChanged(object sender, System.EventArgs e)
        {
            Entity ent = sender as Entity;
            if (ent != null)
            {
                if (ent is DrawAble)
                {
                    DrawAble drawAble = ent as DrawAble;
                    if (drawAble.IsVisable && !drawAbleEntities.Contains(drawAble))
                        drawAbleEntities.Add(drawAble);
                    else if (!drawAble.IsVisable)
                        drawAbleEntities.Remove(drawAble);
                }
            }
        }

        private void Entity_OnActivStateChanged(object sender, System.EventArgs e)
        {
            Entity ent = sender as Entity;
            if (ent != null)
            {
                if (ent.IsActiv)
                {
                    if (!entities.Contains(ent))
                        entities.Add(ent);

                    if (ent is CollidAble)
                    {
                        CollidAble collidableEnt = ent as CollidAble;
                        if (!collidAbleEntities.Contains(collidableEnt))
                        {
                            collidAbleEntities.Add(collidableEnt);
                            CollisionDetection.ActivCollisionDetection.SignIn(collidableEnt);
                        }
                    }
                }
                else
                {
                    entities.Remove(ent);

                    if (ent is CollidAble)
                    {
                        CollidAble collidableEnt = ent as CollidAble;
                        collidAbleEntities.Remove(collidableEnt);
                        CollisionDetection.ActivCollisionDetection.SignOut(collidableEnt);
                    }
                }
            }
        }

        public void SignOut(Entity entity)
        {
            if(entity != null)
            {
                entities.Remove(entity);

                if (entity is DrawAble)
                    drawAbleEntities.Remove(entity as DrawAble);

                if (entity is CollidAble)
                {
                    CollidAble col = entity as CollidAble;

                    collidAbleEntities.Remove(col);
                    CollisionDetection.ActivCollisionDetection.SignOut(col);
                }
            }
        }

        public Entity GetEntityByName(string name)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].Name == name)
                    return entities[i];
            }

            return null;
        }

        public T GetEntityByName<T>(string name) where T : Entity
        {
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].Name == name)
                    return (T)entities[i];
            }

            return null;
        }

        public T GetEntityByType<T>() where T : Entity
        {
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i] is T)
                    return (T)entities[i];
            }

            return null;
        }

        public Entity[] GetEntitiesByName(string name)
        {
            List<Entity> matches = new List<Entity>();
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].Name == name)
                    matches.Add(entities[i]);
            }

            return matches.ToArray();
        }

        public T[] GetEntitiesByName<T>(string name) where T : Entity
        {
            List<T> matches = new List<T>();
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].Name == name)
                    matches.Add((T)entities[i]);
            }

            return matches.ToArray();
        }

        public T[] GetEntitiesByType<T>() where T : Entity
        {
            List<T> matches = new List<T>();
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i] is T)
                    matches.Add((T)entities[i]);
            }

            return matches.ToArray();
        }
    }
}
