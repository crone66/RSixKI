using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RSixKI
{
    public class InteractivObject : CollidAble
    {
        public InteractivObject(bool blocking, Texture2D texture, int layer, float rotation, Vector2 scale, Color color, string name, Vector2 position, bool isVisable = true, bool isActiv = true, int teamId = -1) : base(blocking, texture, layer, rotation, scale, color, name, position, isVisable, isActiv, teamId)
        {

        }

        public InteractivObject(bool blocking, Texture2D texture, int layer, float rotation, Vector2 scale, Color color, string name, Vector2 position, IEnumerable<EntityComponent> components, bool isVisable = true, bool isActiv = true, int teamId = -1) : base(blocking, texture, layer, rotation, scale, color, name, position, components, isVisable, isActiv, teamId)
        {

        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
