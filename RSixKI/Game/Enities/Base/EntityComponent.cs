using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSixKI
{
    public abstract class EntityComponent
    {
        protected string name;
        public Entity parent;

        public string Name
        {
            get
            {
                return name;
            }
        }

        public Entity Parent
        {
            get { return parent; }
        }

        public EntityComponent(Entity parent, string name)
        {
            this.name = name;
            this.parent = parent;
        }
        
        public abstract void Update(GameTime gameTime);
    }
}
