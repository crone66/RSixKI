using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSixKI
{
    public class CollisionDetection
    {
        public static CollisionDetection ActivCollisionDetection;

        private CollisionGrid[] grids;
        private List<CollidAble> collisionObjects;

        public CollisionDetection(CollisionGrid[] grids)
        {
            ActivCollisionDetection = this;

            this.grids = grids;
            collisionObjects = new List<CollidAble>();
        }

        public CollisionGrid[] SignIn(CollidAble collidable)
        {
            if (collidable != null)
            {
                if (!collisionObjects.Contains(collidable))
                    collisionObjects.Add(collidable);

                List<CollisionGrid> grids = CheckGridIntersections(collidable);
                for (int i = 0; i < grids.Count; i++)
                {
                    grids[i].CollisionObjects.Add(collidable);
                }

                collidable.Grids = grids.ToArray();

                return grids.ToArray();
            }
            return null;
        }

        public void SignOut(CollidAble collidable)
        {
            if (collidable != null)
            {
                collisionObjects.Remove(collidable);
                if (collidable.Grids != null)
                {
                    for (int i = 0; i < collidable.Grids.Length; i++)
                    {
                        collidable.Grids[i].CollisionObjects.Remove(collidable);
                    }
                }
            }
        }

        public List<CollidAble> CheckCollision(CollidAble collidable)
        {
            CollisionGrid[] grids = collidable.Grids;
            List<CollidAble> collisions = new List<CollidAble>();
            for (int i = 0; i < grids.Length; i++)
            {
                for (int j = 0; j < grids[i].CollisionObjects.Count; j++)
                {
                    if(collidable != grids[i].CollisionObjects[j])
                    {
                        if(collidable.CollisionBox.Intersects(grids[i].CollisionObjects[j].CollisionBox))
                        {
                            collisions.Add(grids[i].CollisionObjects[j]);
                        }
                    }
                }
            }

            return collisions;        
        }

        public CollisionGrid[] UpdateGrids(CollidAble collidable)
        {
            List<CollisionGrid> newGrids = CheckGridIntersections(collidable);
            List<int> alreadyExists = new List<int>();
            //Remove
            for (int i = 0; i < collidable.Grids.Length; i++)
            {
                if(!newGrids.Contains(collidable.Grids[i]))
                {
                    collidable.Grids[i].CollisionObjects.Remove(collidable);
                }
                else
                {
                    alreadyExists.Add(i);
                }
            }

            //Add
            for (int i = 0; i < newGrids.Count; i++)
            {
                if (!alreadyExists.Contains(i))
                    newGrids[i].CollisionObjects.Add(collidable);
            }

            return newGrids.ToArray();
        }

        private List<CollisionGrid> CheckGridIntersections(CollidAble collidable)
        {
            List<CollisionGrid> collisions = new List<CollisionGrid>();
            for (int i = 0; i < grids.Length; i++)
            {
                if (collidable.CollisionBox.Intersects(grids[i].CollisionBox))
                {
                    collisions.Add(grids[i]);
                }
            }

            return collisions;
        }
    }
}
