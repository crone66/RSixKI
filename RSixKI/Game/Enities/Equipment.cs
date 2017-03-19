using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RSixKI
{
    public abstract class Equipment : Entity
    {
        public enum EquipmentType
        {
            Useable,
            Placeable,
        }

        protected int count;
        protected EquipmentType equipType;

        public Equipment(string name, Vector2 position, EquipmentType equipTyp, int count = 1, bool isActiv = true, int teamId = -1) : base(name, position, isActiv, teamId)
        {
            this.count = count;
            this.equipType = equipTyp;
        }

        public Equipment(string name, Vector2 position, IEnumerable<EntityComponent> components, EquipmentType equipTyp, int count = 1, bool isActiv = true, int teamId = -1) : base(name, position, components, isActiv, teamId)
        {
            this.count = count;
            this.equipType = equipTyp;
        }
    }
}
