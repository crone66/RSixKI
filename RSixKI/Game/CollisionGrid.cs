using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace RSixKI
{
    public class CollisionGrid
    {
        private Rectangle collisionBox;
        private List<CollidAble> collisionObjects;

        public List<CollidAble> CollisionObjects
        {
            get
            {
                return collisionObjects;
            }
        }

        public Rectangle CollisionBox
        {
            get
            {
                return collisionBox;
            }
        }

        public CollisionGrid(Rectangle collisionBox)
        {
            this.collisionBox = collisionBox;
            collisionObjects = new List<CollidAble>();
        }
        
    }
}
