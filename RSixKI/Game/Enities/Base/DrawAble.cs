using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace RSixKI
{
    public abstract class DrawAble : Entity
    {
        private Vector2 center;
        protected int layer;
        private bool isVisable;
        protected Texture2D texture;
        private float rotation;
        protected Vector2 scale;
        protected Color color;
        private Vector2 origin;
        public event EventHandler OnVisabillityChanged;

        public bool IsVisable
        {
            get
            {
                return isVisable;
            }

            set
            {
                isVisable = value;
                OnVisabillityChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        protected float Rotation
        {
            get
            {
                return rotation;
            }

            set
            {
                rotation = Helpers.NormalizeFloat(value, 0, 359);
            }
        }

        public Vector2 Center
        {
            get
            {
                return center;
            }
        }

        public DrawAble(Texture2D texture, int layer, float rotation, Vector2 scale, Color color, string name, Vector2 position, bool isVisable = true, bool isActiv = true, int teamId = -1) : base(name, position, isActiv, teamId)
        {
            OnSpawned += DrawAble_OnSpawned;
            this.texture = texture;
            this.layer = layer;
            this.Rotation = rotation;
            this.scale = scale;
            this.color = color;
            if(texture != null)
                origin = new Vector2(texture.Width / 2, texture.Height / 2);
            IsVisable = isVisable;
            center = Position + origin;
            OnMoved += DrawAble_OnMoved;
        }

        public DrawAble(Texture2D texture, int layer, float rotation, Vector2 origin, Vector2 scale, Color color, string name, Vector2 position, bool isVisable = true, bool isActiv = true, int teamId = -1) : base(name, position, isActiv, teamId)
        {
            OnSpawned += DrawAble_OnSpawned;
            this.texture = texture;
            this.layer = layer;
            this.Rotation = rotation;
            this.scale = scale;
            this.color = color;
            this.origin = origin;
            IsVisable = isVisable;
            center = Position + origin;
            OnMoved += DrawAble_OnMoved;
        }

        public DrawAble(Texture2D texture, int layer, float rotation, Vector2 scale, Color color, string name, Vector2 position, IEnumerable<EntityComponent> components, bool isVisable = true, bool isActiv = true, int teamId = -1) : base(name, position, components, isActiv, teamId)
        {
            OnSpawned += DrawAble_OnSpawned;
            this.texture = texture;
            this.layer = layer;
            this.Rotation = rotation;
            this.scale = scale;
            this.color = color;
            if (texture != null)
                origin = new Vector2(texture.Width / 2, texture.Height / 2);
            IsVisable = isVisable;
            center = Position + origin;
            OnMoved += DrawAble_OnMoved;
        }

        public DrawAble(Texture2D texture, int layer, float rotation, Vector2 origin, Vector2 scale, Color color, string name, Vector2 position, IEnumerable<EntityComponent> components, bool isVisable = true, bool isActiv = true, int teamId = -1) : base(name, position, components, isActiv, teamId)
        {
            OnSpawned += DrawAble_OnSpawned;
            this.texture = texture;
            this.layer = layer;
            this.Rotation = rotation;
            this.scale = scale;
            this.color = color;
            this.origin = origin;
            IsVisable = isVisable;
            center = Position + origin;
            OnMoved += DrawAble_OnMoved;
        }


        private void DrawAble_OnMoved(object sender, MovedArgs e)
        {
            center = e.Position + origin;
        }

        private void DrawAble_OnSpawned(object sender, EventArgs e)
        {
            IsVisable = true;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if(texture != null)
                spriteBatch.Draw(texture, Position + origin, null, color, MathHelper.ToRadians(Rotation), origin, scale, SpriteEffects.None, layer);
        }

        public abstract override object Clone();
    }
}
