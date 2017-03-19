using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSixKI
{
    public class CollisionArgs
    {
        public CollidAble CollisionObject;
        public CollisionArgs(CollidAble collisionObject)
        {
            CollisionObject = collisionObject;
        }
    }
}
