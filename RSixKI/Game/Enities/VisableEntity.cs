using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RSixKI
{
    public class VisableEntity : DrawAble
    {
        public VisableEntity(Texture2D texture, int layer, float rotation, Vector2 scale, Color color, string name, Vector2 position, bool isVisable = true, bool isActiv = false, int teamId = -1) : base(texture, layer, rotation, scale, color, name, position, isVisable, isActiv, teamId)
        {
        }

        public VisableEntity(Texture2D texture, int layer, float rotation, Vector2 scale, Color color, string name, Vector2 position, IEnumerable<EntityComponent> components, bool isVisable = true, bool isActiv = false, int teamId = -1) : base(texture, layer, rotation, scale, color, name, position, components, isVisable, isActiv, teamId)
        {
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
